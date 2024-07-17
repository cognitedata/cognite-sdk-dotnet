module Tests.Integration.SimulatorModels

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Alpha
open Tests.Integration.Alpha.Common

let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorModels")>]
let ``Create and list simulator models is Ok`` () =
    task {
        // Arrange
        let simulatorExternalId = $"test_sim_3_{now}"
        let modelExternalId = $"test_model_{now}"

        let! dataSetRes = bluefieldClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            testSimulatorCreate(simulatorExternalId)

        let modelToCreate =
            SimulatorModelCreate(
                ExternalId = modelExternalId,
                SimulatorExternalId = simulatorExternalId,
                Name = "test_model",
                Description = "test_model_description",
                Labels = [ new CogniteExternalId("test_label") ],
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        try
            // Act
            let! _ = bluefieldClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! modelCreateRes = bluefieldClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let! modelsListRes =
                bluefieldClient.Alpha.Simulators.ListSimulatorModelsAsync(
                    new SimulatorModelQuery(
                        Filter = SimulatorModelFilter(SimulatorExternalIds = [ simulatorExternalId ])
                    )
                )

            let! modelRetriveRes =
                bluefieldClient.Alpha.Simulators.RetrieveSimulatorModelsAsync([ new Identity(modelExternalId) ])

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

            let! modelUpdateRes = bluefieldClient.Alpha.Simulators.UpdateSimulatorModelsAsync([ modelPatch ])
            let updatedModel = modelUpdateRes |> Seq.head
            let! _ = bluefieldClient.Alpha.Simulators.DeleteSimulatorModelsAsync([ new Identity(modelExternalId) ])

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
            bluefieldClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorModels")>]
let ``Create and list simulator model revisions is Ok`` () =
    task {
        // Arrange
        let simulatorExternalId = $"test_sim_3_{now}"
        let modelExternalId = $"test_model_{now}"

        let simulatorToCreate =
            testSimulatorCreate(simulatorExternalId)


        let! dataSetRes = bluefieldClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let fileToCreate =
            FileCreate(
                ExternalId = $"model_revision_file_{now}",
                Name = "test_file_for_model_revision.json",
                MimeType = "application/json",
                Source = "test_source",
                DataSetId = dataSet.Id
            )

        let! fileCreated = bluefieldClient.Files.UploadAsync(fileToCreate)

        let modelToCreate =
            SimulatorModelCreate(
                ExternalId = modelExternalId,
                SimulatorExternalId = simulatorExternalId,
                Name = "test_model",
                Description = "test_model_description",
                Labels = [ new CogniteExternalId("test_label") ],
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        let modelRevisionToCreate =
            SimulatorModelRevisionCreate(
                ExternalId = "test_model_revision",
                ModelExternalId = modelExternalId,
                Description = "test_model_revision_description",
                FileId = fileCreated.Id
            )

        try
            // Act
            let! _ = bluefieldClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! _ = bluefieldClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let! modelRevisionRes =
                bluefieldClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate ])

            let! modelRevisionListRes =
                bluefieldClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
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

            let! modelRevisionRetrieveRes =
                bluefieldClient.Alpha.Simulators.RetrieveSimulatorModelRevisionsAsync(
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
                            Status = Update(SimulatorModelRevisionStatus.failure),
                            StatusMessage = Update<string>("test")
                        )
                )

            let! modelRevisionUpdateRes =
                bluefieldClient.Alpha.Simulators.UpdateSimulatorModelRevisionsAsync([ modelRevisionPatch ])

            let modelRevisionUpdated = modelRevisionUpdateRes |> Seq.head

            // Assert
            test <@ modelRevisionCreated.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionCreated.ModelExternalId = modelRevisionToCreate.ModelExternalId @>
            test <@ modelRevisionCreated.Description = modelRevisionToCreate.Description @>
            test <@ modelRevisionCreated.Status = SimulatorModelRevisionStatus.unknown @>
            test <@ modelRevisionCreated.DataSetId = dataSet.Id @>
            test <@ modelRevisionCreated.FileId = fileCreated.Id @>
            test <@ modelRevisionCreated.VersionNumber = 1 @>
            test <@ modelRevisionCreated.SimulatorExternalId = simulatorExternalId @>
            test <@ isNull modelRevisionCreated.StatusMessage @>

            test <@ modelRevisionFound.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionRetrieved.ExternalId = modelRevisionToCreate.ExternalId @>

            test <@ modelRevisionUpdated.Status = SimulatorModelRevisionStatus.failure @>
            test <@ modelRevisionUpdated.StatusMessage = "test" @>


        finally
            bluefieldClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore

            bluefieldClient.Files.DeleteAsync([ new Identity(fileToCreate.ExternalId) ])
            |> ignore
    }
