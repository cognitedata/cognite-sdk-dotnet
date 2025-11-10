module Tests.Integration.SimulatorModels

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Alpha
open Tests.Integration.Alpha.Common
open System.Collections.Generic
open System.Net.Http


[<Fact>]
[<Trait("resource", "simulatorModels")>]
[<Trait("api", "simulators")>]
let ``Create and list simulator models is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        let simulatorExternalId = $"test_sim_3_{now}"
        let modelExternalId = $"test_model_{now}"

        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate = testSimulatorCreate (simulatorExternalId)

        let modelToCreate =
            SimulatorModelCreate(
                ExternalId = modelExternalId,
                SimulatorExternalId = simulatorExternalId,
                Name = "test_model",
                Description = "test_model_description",
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        try
            // Act
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! modelCreateRes = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let! modelsListRes =
                writeClient.Alpha.Simulators.ListSimulatorModelsAsync(
                    new SimulatorModelQuery(
                        Filter = SimulatorModelFilter(SimulatorExternalIds = [ simulatorExternalId ])
                    )
                )

            let! modelRetriveRes =
                writeClient.Alpha.Simulators.RetrieveSimulatorModelsAsync([ new Identity(modelExternalId) ])

            let modelRetrieved = modelRetriveRes |> Seq.head

            let modelCreated = modelCreateRes |> Seq.head

            let foundCreatedModel =
                modelsListRes.Items |> Seq.find (fun item -> item.ExternalId = modelExternalId)

            let modelPatch =
                new SimulatorModelUpdateItem(
                    id = modelCreated.Id,
                    Update =
                        SimulatorModelUpdate(
                            Description = Update<string>("test_model_description_updated"),
                            Name = Update<string>("test_model_updated")
                        )
                )


            let! modelUpdateRes = writeClient.Alpha.Simulators.UpdateSimulatorModelsAsync([ modelPatch ])
            let updatedModel = modelUpdateRes |> Seq.head
            let! _ = writeClient.Alpha.Simulators.DeleteSimulatorModelsAsync([ new Identity(modelExternalId) ])

            // Assert
            test <@ modelCreated.ExternalId = modelToCreate.ExternalId @>
            test <@ modelCreated.SimulatorExternalId = modelToCreate.SimulatorExternalId @>
            test <@ modelCreated.Name = modelToCreate.Name @>
            test <@ modelCreated.Description = modelToCreate.Description @>
            test <@ modelCreated.DataSetId = modelToCreate.DataSetId @>

            test <@ foundCreatedModel.ExternalId = modelToCreate.ExternalId @>
            test <@ foundCreatedModel.SimulatorExternalId = modelToCreate.SimulatorExternalId @>
            test <@ foundCreatedModel.Name = modelToCreate.Name @>
            test <@ foundCreatedModel.Description = modelToCreate.Description @>
            test <@ foundCreatedModel.DataSetId = modelToCreate.DataSetId @>

            test <@ modelRetrieved.ExternalId = modelToCreate.ExternalId @>

            test <@ updatedModel.Description = modelPatch.Update.Description.Set @>
            test <@ updatedModel.Name = modelPatch.Update.Name.Set @>
        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<Fact>]
[<Trait("resource", "simulatorModels")>]
[<Trait("api", "simulators")>]
let ``Create and list simulator model revisions along with revision data is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        let simulatorExternalId = $"test_sim_3_{now}"
        let modelExternalId = $"test_model_{now}"

        let simulatorToCreate = testSimulatorCreate (simulatorExternalId)


        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let! testFileRes = writeClient.Files.RetrieveAsync([ new Identity("empty.json") ])
        let testFileId = testFileRes |> Seq.head |> (fun f -> f.Id)

        let modelToCreate =
            SimulatorModelCreate(
                ExternalId = modelExternalId,
                SimulatorExternalId = simulatorExternalId,
                Name = "test_model",
                Description = "test_model_description",
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        let modelRevisionToCreate =
            SimulatorModelRevisionCreate(
                ExternalId = "test_model_revision",
                ModelExternalId = modelExternalId,
                Description = "test_model_revision_description",
                FileId = testFileId
            )



        try
            // Act
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let! modelRevisionRes =
                writeClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate ])

            let! modelRevisionListRes =
                writeClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
                    new SimulatorModelRevisionQuery(
                        Filter =
                            SimulatorModelRevisionFilter(
                                ModelExternalIds = [ modelExternalId ],
                                CreatedTime = TimeRange(Min = now - 10000L, Max = now + 10000L),
                                LastUpdatedTime = TimeRange(Min = 0L)
                            ),
                        Sort = [ new SimulatorSortItem(Property = "createdTime", Order = SimulatorSortOrder.desc) ]
                    )
                )

            let! (modelRevisionListResCursor: IItemsWithCursor<SimulatorModelRevision>) =
                writeClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
                    new SimulatorModelRevisionQuery(Limit = 1)
                )

            test <@ isNull modelRevisionListResCursor.NextCursor |> not @>

            let! modelRevisionRetrieveRes =
                writeClient.Alpha.Simulators.RetrieveSimulatorModelRevisionsAsync(
                    [ new Identity(modelRevisionToCreate.ExternalId) ]
                )

            let modelRevisionRetrieved = modelRevisionRetrieveRes |> Seq.head

            let modelRevisionFound =
                modelRevisionListRes.Items
                |> Seq.find (fun item -> item.ExternalId = modelRevisionToCreate.ExternalId)

            let modelRevisionCreated = modelRevisionRes |> Seq.head

            let modelRevisionPatch =
                new SimulatorModelRevisionUpdateItem(
                    id = modelRevisionCreated.Id,
                    Update =
                        SimulatorModelRevisionUpdate(
                            Status = Update SimulatorModelRevisionStatus.failure,
                            StatusMessage = Update<string> "test"
                        )
                )

            // Create test revision data
            let modelRevisionDataInfo = Dictionary(dict [ "key1", "value1"; "key2", "value2" ])

            let flowsheetData =
                [|SimulatorModelRevisionDataFlowsheet(
                    Thermodynamics =
                        SimulatorModelRevisionDataThermodynamic(
                            Components = [ "water"; "oil" ],
                            PropertyPackages = [ "test_property_package" ]
                        ),
                    SimulatorObjectNodes =
                        [ SimulatorModelRevisionDataObjectNode(
                              Id = "test_object_1",
                              Name = "Test Object",
                              Type = "test_type",
                              Properties =
                                  [ SimulatorModelRevisionDataProperty(
                                        Name = "test_property",
                                        Value = SimulatorValue.Create "test_value",
                                        ValueType = SimulatorValueType.STRING,
                                        ReferenceObject = (dict [ "key", "value" ] |> Dictionary),
                                        Unit = SimulatorValueUnitReference(Name = "F", Quantity = "Temperature")
                                    ) ],
                              GraphicalObject =
                                  SimulatorModelRevisionDataGraphicalObject(
                                      Position = SimulatorModelRevisionDataPosition(X = 100.0, Y = 200.0),
                                      Height = Nullable 50.0,
                                      Width = Nullable 100.0,
                                      Angle = Nullable 0.0,
                                      Active = true
                                  )
                          ) ],
                    SimulatorObjectEdges =
                        [ SimulatorModelRevisionDataObjectEdge(
                              Id = "test_edge",
                              Name = "Test Edge",
                              SourceId = "test_object_1",
                              TargetId = "test_object_2",
                              ConnectionType = SimulatorModelRevisionDataConnectionType.Energy
                          ) ]
                )|]

            let dataUpdate =
                new SimulatorModelRevisionDataUpdateItem(
                    ModelRevisionExternalId = modelRevisionCreated.ExternalId,
                    Update =
                        SimulatorModelRevisionDataUpdate(
                            Flowsheets = UpdateNullable flowsheetData,
                            Info = UpdateNullable modelRevisionDataInfo
                        )
                )

            let! _test =
                writeClient.Alpha.Simulators.RetrieveSimulatorModelRevisionDataAsync modelRevisionCreated.ExternalId

            let! modelRevisionDataUpdateRes =
                writeClient.Alpha.Simulators.UpdateSimulatorModelRevisionDataAsync [ dataUpdate ]

            let! modelRevisionUpdatedData =
                writeClient.Alpha.Simulators.RetrieveSimulatorModelRevisionDataAsync modelRevisionCreated.ExternalId

            let! modelRevisionUpdateRes =
                writeClient.Alpha.Simulators.UpdateSimulatorModelRevisionsAsync([ modelRevisionPatch ])

            let modelRevisionUpdated = modelRevisionUpdateRes |> Seq.head

            // Assert
            test <@ modelRevisionCreated.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionCreated.ModelExternalId = modelRevisionToCreate.ModelExternalId @>
            test <@ modelRevisionCreated.Description = modelRevisionToCreate.Description @>
            test <@ modelRevisionCreated.Status = SimulatorModelRevisionStatus.unknown @>
            test <@ modelRevisionCreated.DataSetId = dataSet.Id @>
            test <@ modelRevisionCreated.FileId = testFileId @>
            test <@ modelRevisionCreated.VersionNumber = 1 @>
            test <@ modelRevisionCreated.SimulatorExternalId = simulatorExternalId @>
            test <@ isNull modelRevisionCreated.StatusMessage @>

            test <@ modelRevisionFound.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionRetrieved.ExternalId = modelRevisionToCreate.ExternalId @>

            test <@ modelRevisionUpdated.Status = SimulatorModelRevisionStatus.failure @>
            test <@ modelRevisionUpdated.StatusMessage = "test" @>
            test <@ modelRevisionUpdatedData.Info.["key1"] = "value1" @>
            test <@ modelRevisionUpdatedData.Info.ToString() = modelRevisionDataInfo.ToString() @>

            let updatedFlowsheet = modelRevisionUpdatedData.Flowsheets |> Seq.head
            let flowsheetDataItem = flowsheetData |> Seq.head

            test
                <@
                    List.ofSeq updatedFlowsheet.Thermodynamics.Components = List.ofSeq
                        [ "water"; "oil" ]
                @>

            test <@ updatedFlowsheet.ToString() = flowsheetDataItem.ToString() @>


        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<Fact>]
[<Trait("resource", "simulatorModels")>]
[<Trait("api", "simulators")>]
let ``List simulator model revisions with SimulatorExternalIds filter is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        let simulatorExternalId1 = $"test_sim_filter_1_{now}"
        let simulatorExternalId2 = $"test_sim_filter_2_{now}"
        let modelExternalId1 = $"test_model_filter_1_{now}"
        let modelExternalId2 = $"test_model_filter_2_{now}"

        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let! testFileRes = writeClient.Files.RetrieveAsync([ new Identity("empty.json") ])
        let testFileId = testFileRes |> Seq.head |> (fun f -> f.Id)

        let simulatorToCreate1 = testSimulatorCreate (simulatorExternalId1)
        let simulatorToCreate2 = testSimulatorCreate (simulatorExternalId2)

        let modelToCreate1 =
            SimulatorModelCreate(
                ExternalId = modelExternalId1,
                SimulatorExternalId = simulatorExternalId1,
                Name = "test_model_1",
                Description = "test_model_1_description",
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        let modelToCreate2 =
            SimulatorModelCreate(
                ExternalId = modelExternalId2,
                SimulatorExternalId = simulatorExternalId2,
                Name = "test_model_2",
                Description = "test_model_2_description",
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        let modelRevisionToCreate1 =
            SimulatorModelRevisionCreate(
                ExternalId = $"test_model_revision_1_{now}",
                ModelExternalId = modelExternalId1,
                Description = "test_model_revision_1_description",
                FileId = testFileId
            )

        let modelRevisionToCreate2 =
            SimulatorModelRevisionCreate(
                ExternalId = $"test_model_revision_2_{now}",
                ModelExternalId = modelExternalId2,
                Description = "test_model_revision_2_description",
                FileId = testFileId
            )

        try
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate1 ])
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate2 ])

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate1 ])
            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate2 ])

            let! _ =
                writeClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate1 ])

            let! _ =
                writeClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate2 ])

            let! modelRevisionListRes1 =
                writeClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
                    new SimulatorModelRevisionQuery(
                        Filter =
                            SimulatorModelRevisionFilter(SimulatorExternalIds = [ simulatorExternalId1 ])
                    )
                )

            let! modelRevisionListRes2 =
                writeClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
                    new SimulatorModelRevisionQuery(
                        Filter =
                            SimulatorModelRevisionFilter(
                                SimulatorExternalIds = [ simulatorExternalId1; simulatorExternalId2 ]
                            )
                    )
                )

            let! modelRevisionListRes3 =
                writeClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
                    new SimulatorModelRevisionQuery(
                        Filter =
                            SimulatorModelRevisionFilter(
                                SimulatorExternalIds = [ simulatorExternalId1 ],
                                ModelExternalIds = [ modelExternalId1 ]
                            )
                    )
                )

            // Assert
            let revisions1 = modelRevisionListRes1.Items |> Seq.toList

            test <@ revisions1.Length = 1 @>
            test <@ revisions1.[0].SimulatorExternalId = simulatorExternalId1 @>
            test <@ revisions1.[0].ModelExternalId = modelExternalId1 @>

            let revisions2 = modelRevisionListRes2.Items |> Seq.toList

            test <@ revisions2.Length = 2 @>
            
            let revision1 = revisions2 |> List.find (fun r -> r.SimulatorExternalId = simulatorExternalId1)
            let revision2 = revisions2 |> List.find (fun r -> r.SimulatorExternalId = simulatorExternalId2)
            
            test <@ revision1.ModelExternalId = modelExternalId1 @>
            test <@ revision2.ModelExternalId = modelExternalId2 @>
            
            let revisions3 = modelRevisionListRes3.Items |> Seq.toList

            test <@ revisions3.Length = 1 @>
            test <@ revisions3.[0].SimulatorExternalId = simulatorExternalId1 @>
            test <@ revisions3.[0].ModelExternalId = modelExternalId1 @>

        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId1) ])
            |> ignore

            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId2) ])
            |> ignore
    }