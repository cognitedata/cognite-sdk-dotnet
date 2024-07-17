module Tests.Integration.Simulators

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Alpha
open Tests.Integration.Alpha.Common

let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
let simulatorExternalId = $"test_sim_{now}"

[<Fact>]
// [<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
let ``Create and delete simulators is Ok`` () =
    task {

        let fileExtensionTypes = seq { "json" }
        // Arrange
        let itemToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = fileExtensionTypes
            )

        // Act
        let! res = writeClient.Alpha.Simulators.CreateAsync([ itemToCreate ])
        let! _ = writeClient.Alpha.Simulators.DeleteAsync [ new Identity(itemToCreate.ExternalId) ]

        // Assert
        let len = Seq.length res
        test <@ len = 1 @>
        let itemRes = res |> Seq.head

        test <@ itemRes.Name = itemToCreate.Name @>
        test <@ (List.ofSeq itemRes.FileExtensionTypes) = (List.ofSeq fileExtensionTypes) @>
        test <@ itemRes.CreatedTime >= now @>
        test <@ itemRes.LastUpdatedTime >= now @>

        ()
    }

[<Fact>]
// [<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
let ``List simulators is Ok`` () =
    task {

        // Arrange
        let query = SimulatorQuery(Filter = SimulatorFilter())

        // Act
        let! res = writeClient.Alpha.Simulators.ListAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len > 0 @>

        test <@ res.Items |> Seq.forall (fun item -> item.Name <> null) @>
    }

[<Fact>]
// [<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
let ``Create and update simulator integration is Ok`` () =
    task {
        // Arrange
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let simulatorToCreate =
            SimulatorCreate(ExternalId = simulatorExternalId, Name = "test_sim", FileExtensionTypes = [ "json" ])

        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = dataSet.Id,
                SimulatorVersion = "N/A",
                ConnectorVersion = "1.2.3"
            )

        try
            // Act
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! integrationCreateRes =
                writeClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])

            let integrationCreated = integrationCreateRes |> Seq.head

            let integrationToUpdate =
                new SimulatorIntegrationUpdateItem(
                    id = integrationCreated.Id,
                    Update =
                        SimulatorIntegrationUpdate(
                            ConnectorStatus = Update<string>("test"),
                            SimulatorVersion = Update<string>("2.3.4"),
                            LicenseStatus = Update<string>("Good"),
                            LicenseLastCheckedTime = Update<int64>(now),
                            ConnectorStatusUpdatedTime = Update<int64>(now)
                        )
                )

            let! integrationUpdateRes =
                writeClient.Alpha.Simulators.UpdateSimulatorIntegrationAsync([ integrationToUpdate ])

            let integrationUpdated = integrationUpdateRes |> Seq.head

            // Assert
            let integration = integrationCreateRes |> Seq.head

            test <@ integration.ExternalId = integrationToCreate.ExternalId @>
            test <@ integration.ConnectorVersion = integrationToCreate.ConnectorVersion @>
            test <@ integration.SimulatorExternalId = integrationToCreate.SimulatorExternalId @>
            test <@ integration.CreatedTime >= now @>
            test <@ integration.LastUpdatedTime >= now @>
            test <@ integration.LicenseLastCheckedTime = Nullable() @>
            test <@ integration.ConnectorStatusUpdatedTime = Nullable() @>

            test <@ integrationUpdated.ConnectorStatus = "test" @>
            test <@ integrationUpdated.SimulatorVersion = "2.3.4" @>
            test <@ integrationUpdated.ConnectorStatusUpdatedTime = Nullable now @>
            test <@ integrationUpdated.LicenseLastCheckedTime = Nullable now @>
        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<Fact>]
// [<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
let ``List simulator integrations is Ok`` () =
    task {
        // Arrange
        let query = SimulatorIntegrationQuery()

        // Act
        let! res = writeClient.Alpha.Simulators.ListSimulatorIntegrationsAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len >= 0 @>
    }

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
            SimulatorCreate(ExternalId = simulatorExternalId, Name = "test_sim", FileExtensionTypes = [ "json" ])

        let modelToCreate =
            SimulatorModelCreate(
                ExternalId = modelExternalId,
                SimulatorExternalId = simulatorExternalId,
                Name = "test_model",
                Description = "test_model_description",
                Labels = [ new CogniteExternalId("test_label") ],
                DataSetId = dataSet.Id
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
            SimulatorCreate(ExternalId = simulatorExternalId, Name = "test_sim", FileExtensionTypes = [ "json" ])


        let! dataSetRes = bluefieldClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let fileToCreate =
            FileCreate(
                ExternalId = $"model_revision_file_{now}",
                Name = "test_file_for_model_revision",
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
                DataSetId = dataSet.Id
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
