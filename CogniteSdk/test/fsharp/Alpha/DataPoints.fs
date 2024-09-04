module Tests.Integration.Alpha.Datapoints

open System

open FSharp.Control.TaskBuilder
open Xunit
open Com.Cognite.V1.Timeseries.Proto

open CogniteSdk.Alpha

open Tests.Integration.Common
open CogniteSdk.Beta.DataModels
open CogniteSdk.Beta.DataModels.Core

let testSpace = "dotnet-sdk-integration-test-space"

module Fixtures =
    type DMFixture() =
        do
            writeClient.Beta.DataModels
                .UpsertSpaces([ SpaceCreate(Space = testSpace) ])
                .GetAwaiter()
                .GetResult()
            |> ignore

        interface IDisposable with
            member __.Dispose() =
                writeClient.Beta.DataModels.DeleteSpaces([ testSpace ]).GetAwaiter().GetResult()
                |> ignore

                ()

module DataPointsTests =
    type DataPointsTests() =
        [<Fact>]
        member __.``Insert datapoints is Ok``() =
            task {
                let externalIdString = Guid.NewGuid().ToString()


                let dto =
                    SourcedNodeWrite<'CogniteTimeSeriesBase>(
                        Space = testSpace,
                        ExternalId = externalIdString,
                        Properties =
                            CogniteTimeSeriesBase(
                                TimeSeriesType.Numeric,
                                Name = "Insert datapoints test",
                                Description = "dotnet sdk test"
                            )
                    )


                let dataPoints = NumericDatapoints()

                dataPoints.Datapoints.Add(
                    NumericDatapoint(Timestamp = 1563048800000L, Value = 3.0, Status = Status(Symbol = "GoodClamped"))
                )

                let points = DataPointInsertionRequest()

                points.Items.Add
                    [ DataPointInsertionItem(
                          InstanceId = InstanceId(Space = testSpace, ExternalId = externalIdString),
                          NumericDatapoints = dataPoints
                      ) ]

                // Act
                let! _ = writeClient.CoreDataModel.TimeSeries().UpsertAsync([ dto ], null)
                let! _ = writeClient.Alpha.DataPoints.CreateAsync points

                let! _ =
                    writeClient.Alpha.DataPoints.CreateAsync(points, System.IO.Compression.CompressionLevel.Fastest)

                let! _ =
                    writeClient.CoreDataModel.TimeSeries().DeleteAsync
                        [ InstanceIdentifierWithType(InstanceType.node, InstanceIdentifier(testSpace, externalIdString)) ]

                ()
            // Assert
            }

        [<Fact>]
        member __.``Delete datapoints is Ok``() =
            task {
                // Arrange
                let externalIdString = Guid.NewGuid().ToString()

                let dto =
                    SourcedNodeWrite<'CogniteTimeSeriesBase>(
                        Space = testSpace,
                        ExternalId = externalIdString,
                        Properties =
                            CogniteTimeSeriesBase(
                                TimeSeriesType.Numeric,
                                Name = "Delete datapoints test",
                                Description = "dotnet sdk test"
                            )
                    )

                let startTimestamp = 1563048800000L
                let endDeleteTimestamp = 1563048800051L
                let endTimestamp = 1563048800100L

                let dataPoints = NumericDatapoints()

                for ts in startTimestamp..endTimestamp do
                    dataPoints.Datapoints.Add(NumericDatapoint(Timestamp = ts, Value = 1.0))

                let points = DataPointInsertionRequest()

                points.Items.Add
                    [ DataPointInsertionItem(
                          InstanceId = InstanceId(Space = testSpace, ExternalId = externalIdString),
                          NumericDatapoints = dataPoints
                      ) ]

                let delete =
                    DataPointsDelete(
                        Items =
                            [ IdentityWithRange(
                                  InstanceId = InstanceIdentifier(Space = testSpace, ExternalId = externalIdString),
                                  InclusiveBegin = startTimestamp,
                                  ExclusiveEnd = Nullable endDeleteTimestamp
                              ) ]
                    )

                // Act
                let! _ = writeClient.CoreDataModel.TimeSeries().UpsertAsync([ dto ], null)
                let! _ = writeClient.Alpha.DataPoints.CreateAsync points
                let! res = writeClient.Alpha.DataPoints.DeleteAsync delete

                let! _ =
                    writeClient.CoreDataModel.TimeSeries().DeleteAsync
                        [ InstanceIdentifierWithType(InstanceType.node, InstanceIdentifier(testSpace, externalIdString)) ]

                // Assert
                ()
            }


        interface IClassFixture<Fixtures.DMFixture>
