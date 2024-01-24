module Tests.Integration.Simulators

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Alpha

let azureDevClient =
    let oAuth2AccessToken = Environment.GetEnvironmentVariable "TEST_TOKEN_WRITE"

    createOAuth2SdkClient oAuth2AccessToken "charts-azuredev" "https://azure-dev.cognitedata.com"

type FactIf(envVar: string, skipReason: string) =
    inherit FactAttribute()

    override this.Skip =
        let envFlag =
            Environment.GetEnvironmentVariable envVar
            |> Option.ofObj
            |> Option.map (fun x -> x = "true")
            |> Option.defaultValue false

        if envFlag then null else skipReason

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulationRuns")>]
let ``Create simulation runs is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()

        let itemToCreate =
            SimulationRunCreate(
                SimulatorName = "DWSIM",
                ModelName = "ShowerMixerIntegrationTest",
                RoutineName = "ShowerMixerCalculation",
                RunType = SimulationRunType.external,
                ValidationEndTime = now,
                Queue = true
            )

        // Act
        let! res = azureDevClient.Alpha.Simulators.CreateSimulationRunsAsync([ itemToCreate ])

        // Assert
        let len = Seq.length res
        test <@ len = 1 @>
        let itemRes = res |> Seq.head

        test <@ itemRes.SimulatorName = itemToCreate.SimulatorName @>
        test <@ itemRes.ModelName = itemToCreate.ModelName @>
        test <@ itemRes.RoutineName = itemToCreate.RoutineName @>
        test <@ itemRes.Status = SimulationRunStatus.ready @>
        test <@ itemRes.RunType = SimulationRunType.external @>
        test <@ itemRes.ValidationEndTime = Nullable(now) @>
        test <@ now - itemRes.CreatedTime < 10000 @>
        test <@ now - itemRes.LastUpdatedTime < 10000 @>

    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulationRuns")>]
let ``List simulation runs is Ok`` () =
    task {

        // Arrange
        let query =
            SimulationRunQuery(
                Filter = SimulationRunFilter(SimulatorName = "DWSIM", Status = SimulationRunStatus.success)
            )

        // Act
        let! res = azureDevClient.Alpha.Simulators.ListSimulationRunsAsync(query)

        let len = Seq.length res.Items

        test <@ res.Items |> Seq.forall (fun item -> item.SimulatorName = "DWSIM") @>
        test <@ res.Items |> Seq.forall (fun item -> item.Status = SimulationRunStatus.success) @>

        test
            <@
                res.Items
                |> Seq.forall (fun item -> item.CreatedTime > 0 && item.LastUpdatedTime > 0)
            @>

        // Assert
        test <@ len > 0 @>
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulationRuns")>]
let ``Callback simulation runs is Ok`` () =
    task {

        // Arrange
        let listQuery =
            SimulationRunQuery(
                Filter =
                    SimulationRunFilter(
                        SimulatorName = "DWSIM",
                        ModelName = "ShowerMixerIntegrationTest",
                        RoutineName = "ShowerMixerCalculation",
                        Status = SimulationRunStatus.ready
                    )
            )

        let! listRes = azureDevClient.Alpha.Simulators.ListSimulationRunsAsync(listQuery)

        test <@ Seq.length listRes.Items > 0 @>

        let simulationRun = listRes.Items |> Seq.head
        let ts = DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()


        let query =
            SimulationRunCallbackItem(Id = simulationRun.Id, Status = SimulationRunStatus.success, StatusMessage = ts)

        // Act
        let! res = azureDevClient.Alpha.Simulators.SimulationRunCallbackAsync query
        let simulationRunCallbackRes = res.Items |> Seq.head

        // Assert
        test <@ Seq.length res.Items = 1 @>

        test <@ simulationRunCallbackRes.Status = SimulationRunStatus.success @>
        test <@ simulationRunCallbackRes.StatusMessage = ts @>
    }


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulationRuns")>]
let ``Retrieve simulation runs is Ok`` () =
    task {

        // Arrange
        let listQuery = SimulationRunQuery()

        let! listRes = azureDevClient.Alpha.Simulators.ListSimulationRunsAsync(listQuery)

        test <@ Seq.length listRes.Items > 0 @>

        let simulationRun = listRes.Items |> Seq.head

        // Act
        let! res = azureDevClient.Alpha.Simulators.RetrieveSimulationRunsAsync [ simulationRun.Id ]
        let simulationRunRetrieveRes = res |> Seq.head

        // Assert
        test <@ Seq.length res = 1 @>
        test <@ simulationRunRetrieveRes.Status = SimulationRunStatus.success @>
    }

let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
let simulatorExternalId = $"test_sim_{now}"

[<Fact>]
[<Trait("resource", "simulators")>]
let ``Create and delete simulators is Ok`` () =
    task {

        let fileExtensionTypes = seq { "json" }
        // Arrange
        let itemToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = fileExtensionTypes,
                Enabled = true
            )

        // Act
        let! res = writeClient.Alpha.Simulators.CreateAsync([ itemToCreate ])
        let! _ = writeClient.Alpha.Simulators.DeleteAsync [ new Identity(itemToCreate.ExternalId) ]

        // Assert
        let len = Seq.length res
        test <@ len = 1 @>
        let itemRes = res |> Seq.head

        test <@ itemRes.Name = itemToCreate.Name @>
        test <@ itemRes.Enabled = itemToCreate.Enabled.Value @>
        test <@ (List.ofSeq itemRes.FileExtensionTypes) = (List.ofSeq fileExtensionTypes) @>
        test <@ itemRes.CreatedTime >= now @>
        test <@ itemRes.LastUpdatedTime >= now @>

        ()
    }

[<Fact>]
[<Trait("resource", "simulators")>]
let ``List simulators is Ok`` () =
    task {

        // Arrange
        let query = SimulatorQuery(Filter = SimulatorFilter(Enabled = true))

        // Act
        let! res = writeClient.Alpha.Simulators.ListAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len > 0 @>

        test <@ res.Items |> Seq.forall (fun item -> item.Enabled = true) @>
        test <@ res.Items |> Seq.forall (fun item -> item.Name <> null) @>
    }

[<Fact>]
[<Trait("resource", "simulators")>]
let ``Create and update simulator integration is Ok`` () =
    task {
        // Arrange
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let simulatorToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [ "json" ],
                Enabled = true
            )

        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = dataSet.Id,
                SimulatorVersion = "N/A",
                ConnectorVersion = "1.2.3",
                RunApiEnabled = true
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

        let! dataSetRes = azureDevClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [ "json" ],
                Enabled = true
            )

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
            let! _ = azureDevClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! modelCreateRes = azureDevClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let! modelsListRes =
                azureDevClient.Alpha.Simulators.ListSimulatorModelsAsync(
                    new SimulatorModelQuery(
                        Filter = SimulatorModelFilter(SimulatorExternalIds = [ simulatorExternalId ])
                    )
                )

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

            let! modelUpdateRes = azureDevClient.Alpha.Simulators.UpdateSimulatorModelsAsync([ modelPatch ])
            let updatedModel = modelUpdateRes |> Seq.head
            let! _ = azureDevClient.Alpha.Simulators.DeleteSimulatorModelsAsync([ new Identity(modelExternalId) ])

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

            test <@ updatedModel.Description = modelPatch.Update.Description.Set @>
            test <@ updatedModel.Name = modelPatch.Update.Name.Set @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
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
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [ "json" ],
                Enabled = true
            )


        let! dataSetRes = azureDevClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let fileToCreate =
            FileCreate(
                ExternalId = $"model_revision_file_{now}",
                Name = "test_file_for_model_revision",
                MimeType = "application/json",
                Source = "test_source",
                DataSetId = dataSet.Id
            )

        let! fileCreated = azureDevClient.Files.UploadAsync(fileToCreate)

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
            let! _ = azureDevClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! _ = azureDevClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let! modelRevisionRes =
                azureDevClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate ])

            let! modelRevisionListRes =
                azureDevClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
                    new SimulatorModelRevisionQuery(
                        Filter = SimulatorModelRevisionFilter(
                            ModelExternalIds = [ modelExternalId ],
                            CreatedTime = TimeRange(Min = now - 10000L, Max = now + 10000L),
                            LastUpdatedTime = TimeRange(Min = 0L)
                        ),
                        Sort = [ new SimulatorSortItem(Property = "createdTime", Order = SimulatorSortOrder.desc) ]
                    )
                )

            let modelRevisionFound =
                modelRevisionListRes.Items
                |> Seq.find (fun item -> item.ExternalId = modelRevisionToCreate.ExternalId)

            let modelRevisionCreated = modelRevisionRes |> Seq.head

            let modelRevisionPatch =
                new SimulatorModelRevisionUpdateItem(
                    id = modelRevisionCreated.Id,
                    Update =
                        SimulatorModelRevisionUpdate(
                            BoundaryConditionsStatus = Update(SimulatorModelRevisionStatus.success),
                            Status = Update(SimulatorModelRevisionStatus.failure),
                            StatusMessage = Update<string>("test")
                        )
                )

            let! modelRevisionUpdateRes =
                azureDevClient.Alpha.Simulators.UpdateSimulatorModelRevisionsAsync([ modelRevisionPatch ])

            let modelRevisionUpdated = modelRevisionUpdateRes |> Seq.head

            // Assert
            test <@ modelRevisionCreated.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionCreated.ModelExternalId = modelRevisionToCreate.ModelExternalId @>
            test <@ modelRevisionCreated.Description = modelRevisionToCreate.Description @>
            test <@ modelRevisionCreated.Status = SimulatorModelRevisionStatus.unknown @>
            test <@ modelRevisionCreated.BoundaryConditionsStatus = SimulatorModelRevisionStatus.unknown @>
            test <@ modelRevisionCreated.DataSetId = dataSet.Id @>
            test <@ modelRevisionCreated.FileId = fileCreated.Id @>
            test <@ modelRevisionCreated.VersionNumber = 1 @>
            test <@ modelRevisionCreated.SimulatorExternalId = simulatorExternalId @>
            test <@ isNull modelRevisionCreated.StatusMessage @>

            test <@ modelRevisionFound.ExternalId = modelRevisionToCreate.ExternalId @>

            test <@ modelRevisionUpdated.Status = SimulatorModelRevisionStatus.failure @>
            test <@ modelRevisionUpdated.BoundaryConditionsStatus = SimulatorModelRevisionStatus.success @>
            test <@ modelRevisionUpdated.StatusMessage = "test" @>


        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore

            azureDevClient.Files.DeleteAsync([ new Identity(fileToCreate.ExternalId) ])
            |> ignore
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS" , skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorRoutines")>]
let ``Create simulator routines is Ok`` () =
    task {
        // Arrange
        let routineExternalId = $"test_routine_3_{now}"
        let modelExternalId = $"test_model_{now}"
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let! dataSetRes = azureDevClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [ "json" ],
                Enabled = true
            )

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = dataSet.Id,
                SimulatorVersion = "N/A",
                ConnectorVersion = "1.2.3",
                RunApiEnabled = true
            )
        
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
            let! _ = azureDevClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! integrationCreateRes =
                azureDevClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])

            let integrationCreated = integrationCreateRes |> Seq.head

            let! modelCreateRes = azureDevClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])
            
            let modelCreated = modelCreateRes |> Seq.head

            let routineToCreate =
                SimulatorRoutineCreateCommandItem(
                    ExternalId = routineExternalId,
                    ModelExternalId = modelCreated.ExternalId,
                    SimulatorIntegrationExternalId = integrationCreated.ExternalId,
                    Name = "Test"
                )
            
            let routineToCreatePredefined =
                SimulatorRoutineCreateCommandPredefined(
                    ExternalId = routineExternalId+"_predefined",
                    ModelExternalId = modelCreated.ExternalId,
                    SimulatorIntegrationExternalId = integrationCreated.ExternalId,
                    CalculationType = "IPR/VLP"
                )

            // Act
            let! resRoutine = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])
            let! resRoutinePredefined = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesPredefinedAsync([ routineToCreatePredefined ])

            // Assert
            let lenRoutine = Seq.length resRoutine
            let lenPredefinedRoutine = Seq.length resRoutinePredefined
            test <@ lenRoutine = 1 @>
            test <@ lenPredefinedRoutine = 1 @>
            // let itemRes = res |> Seq.head
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS" , skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorRoutineRevisions")>]
let ``Create simulator predefined routine revisions is Ok`` () =
    task {
        // Arrange
        let routineExternalId = $"test_routine_3_{now}_predefined"
        let routineRevisionExternalId = $"{routineExternalId}_1"
        let modelExternalId = $"test_model_{now}"
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let! dataSetRes = azureDevClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [ "json" ],
                Enabled = true
            )

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = dataSet.Id,
                SimulatorVersion = "N/A",
                ConnectorVersion = "1.2.3",
                RunApiEnabled = true
            )
        
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
            let! _ = azureDevClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! integrationCreateRes =
                azureDevClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])

            let integrationCreated = integrationCreateRes |> Seq.head

            let! modelCreateRes = azureDevClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])
            
            let modelCreated = modelCreateRes |> Seq.head
            
            let routineToCreatePredefined =
                SimulatorRoutineCreateCommandPredefined(
                    ExternalId = routineExternalId,
                    ModelExternalId = modelCreated.ExternalId,
                    SimulatorIntegrationExternalId = integrationCreated.ExternalId,
                    CalculationType = "ChokeDp"
                )

            let revisionToCreate  =
                SimulatorRoutineRevisionCreate(
                    ExternalId = routineRevisionExternalId,
                    RoutineExternalId = routineExternalId,
                    Configuration = SimulatorRoutineRevisionConfiguration(
                        Schedule = SimulatorRoutineRevisionSchedule(
                            Enabled = false // TODO: true
                        ),
                        LogicalCheck = SimulatorRoutineRevisionLogicalCheck(
                            Enabled = false // TODO: true
                        ),
                        // TODO rename
                        SteadyStateDetection = SimulatorRoutineRevisionSteadyStateDetection(
                            Enabled = false // TODO: true
                        ),
                        DataSampling = SimulatorRoutineRevisionDataSampling(
                            ValidationWindow = 1,
                            SamplingWindow = 1,
                            Granularity = 1,
                            ValidationEndOffset = "1m"
                        ),
                        InputTimeseries = [],
                        OutputTimeseries = [],
                        OutputSequences = [
                            SimulatorRoutineRevisionOutputSequence(
                                Name = "test",
                                ReferenceId = "test"
                            )
                        ]
                    )
                )

            // Act
            let! resRoutinePredefined = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesPredefinedAsync([ routineToCreatePredefined ])

            let! resRevision = azureDevClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revisionToCreate ])

            // Assert
            // let lenRoutine = Seq.length resRoutine
            let lenPredefinedRoutine = Seq.length resRoutinePredefined
            // test <@ lenRoutine = 1 @>
            test <@ lenPredefinedRoutine = 1 @>
            // let itemRes = res |> Seq.head

            test <@ resRevision |> Seq.length = 1 @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS" , skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorRoutineRevisions")>]
let ``Create simulator routine revisions is Ok`` () =
    task {
        // Arrange
        let routineExternalId = $"test_routine_3_{now}"
        let routineRevisionExternalId = $"{routineExternalId}_1"
        let modelExternalId = $"test_model_{now}"
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let! dataSetRes = azureDevClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [ "json" ],
                Enabled = true
            )

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = dataSet.Id,
                SimulatorVersion = "N/A",
                ConnectorVersion = "1.2.3",
                RunApiEnabled = true
            )
        
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
            let! _ = azureDevClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! integrationCreateRes =
                azureDevClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])

            let integrationCreated = integrationCreateRes |> Seq.head

            let! modelCreateRes = azureDevClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])
            
            let modelCreated = modelCreateRes |> Seq.head

            let routineToCreate =
                SimulatorRoutineCreateCommandItem(
                    ExternalId = routineExternalId,
                    ModelExternalId = modelCreated.ExternalId,
                    SimulatorIntegrationExternalId = integrationCreated.ExternalId,
                    Name = "Test"
                )

            let routineStage =
                SimulatorRoutineRevisionStage(
                    Order = 1,
                    Description = "test",
                    Steps = [
                        SimulatorRoutineRevisionScriptStep(
                            Order = 1,
                            StepType = "Set",
                            Description = "test",
                            Arguments =
                                SimulatorRoutineRevisionArguments(
                                    ArgumentType = "inputTimeSeries", 
                                    ReferenceId = "test",
                                    ObjectName = "test",
                                    ObjectProperty = "test2" 
                                )
                        )
                        SimulatorRoutineRevisionScriptStep(
                            Order = 2,
                            StepType = "Get",
                            Description = "test",
                            Arguments =
                                SimulatorRoutineRevisionArguments(
                                    ArgumentType = "outputTimeSeries", 
                                    ReferenceId = "test",
                                    ObjectName = "test",
                                    ObjectProperty = "test2" 
                                )
                        )
                    ]
                )

            // TODO check backend for mixed case
            let revisionToCreate  =
                SimulatorRoutineRevisionCreate(
                    ExternalId = routineRevisionExternalId,
                    RoutineExternalId = routineExternalId,
                    Script = [routineStage],
                    Configuration = SimulatorRoutineRevisionConfiguration(
                        Schedule = SimulatorRoutineRevisionSchedule(
                            Enabled = false // TODO: true
                        ),
                        LogicalCheck = SimulatorRoutineRevisionLogicalCheck(
                            Enabled = false // TODO: true
                        ),
                        // TODO rename
                        SteadyStateDetection = SimulatorRoutineRevisionSteadyStateDetection(
                            Enabled = false // TODO: true
                        ),
                        DataSampling = SimulatorRoutineRevisionDataSampling(
                            ValidationWindow = 1,
                            SamplingWindow = 1,
                            Granularity = 1,
                            ValidationEndOffset = "1m"
                        ),
                        InputTimeseries = [],
                        OutputTimeseries = [],
                        InputConstants = []
                    )
                )

            // Act
            let! resRoutine = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resRevision = azureDevClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revisionToCreate ])

            let! resListRevisions = azureDevClient.Alpha.Simulators.ListSimulatorRoutineRevisionsAsync(new SimulatorRoutineRevisionQuery())

            let revisionFound = resListRevisions.Items |> Seq.find (fun item -> item.ExternalId = routineRevisionExternalId)

            // Assert
            let lenRoutine = Seq.length resRoutine
            test <@ lenRoutine = 1 @>

            test <@ resRevision |> Seq.length = 1 @>

            test <@ revisionFound.ExternalId = routineRevisionExternalId @>
            test <@ revisionFound.RoutineExternalId = routineExternalId @>
            
            let scriptStage = Seq.head revisionFound.Script

            printfn "%A" scriptStage
            printfn "%A" routineStage // TODO
            // test <@ scriptStage = routineStage @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }
