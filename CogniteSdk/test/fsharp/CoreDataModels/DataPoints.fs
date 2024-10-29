module Tests.Integration.CoreDataModels.Datapoints

open System

open FSharp.Control.TaskBuilder
open Xunit
open Com.Cognite.V1.Timeseries.Proto

open CogniteSdk

open Tests.Integration.Common
open CogniteSdk.DataModels
open CogniteSdk.DataModels.Core

let testSpace = "dotnet-sdk-integration-test-space"

module Fixtures =
    open System.Text.Json.Nodes

    type DMFixture() =
        do
            writeClient.DataModels
                .UpsertSpaces([ SpaceCreate(Space = testSpace) ])
                .GetAwaiter()
                .GetResult()
            |> ignore

        interface IDisposable with
            member __.Dispose() =
                let nodes =
                    writeClient.DataModels
                        .FilterInstances<JsonNode>(
                            InstancesFilter(
                                Filter =
                                    EqualsFilter(
                                        Property = [ "node"; "space" ],
                                        Value = RawPropertyValue(Value = testSpace)
                                    ),
                                Limit = 1000,
                                InstanceType = InstanceType.node
                            )
                        )
                        .GetAwaiter()
                        .GetResult()

                if not <| Seq.isEmpty (nodes.Items) then
                    writeClient.DataModels
                        .DeleteInstances(
                            nodes.Items
                            |> Seq.map (fun x ->
                                let item = new InstanceIdentifierWithType(InstanceType.node, x.Space, x.ExternalId)

                                item)
                        )
                        .GetAwaiter()
                        .GetResult()
                    |> ignore

                writeClient.DataModels.DeleteSpaces([ testSpace ]).GetAwaiter().GetResult()
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
                                Type = TimeSeriesType.Numeric,
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
                let! _ = writeClient.DataPoints.CreateAsync points

                let! _ =
                    writeClient.DataPoints.CreateAsync(points, System.IO.Compression.CompressionLevel.Fastest)

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
                                Type = TimeSeriesType.Numeric,
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
                let! _ = writeClient.DataPoints.CreateAsync points
                let! res = writeClient.DataPoints.DeleteAsync delete

                let! _ =
                    writeClient.CoreDataModel.TimeSeries().DeleteAsync
                        [ InstanceIdentifierWithType(InstanceType.node, InstanceIdentifier(testSpace, externalIdString)) ]

                // Assert
                ()
            }


        interface IClassFixture<Fixtures.DMFixture>
