module Tests.Integration.Simulators

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Alpha
open System.Text.Json
open Tests.Integration.Alpha.Common

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
                azureDevClient.Alpha.Simulators.RetrieveSimulatorModelRevisionsAsync(
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
            test <@ modelRevisionRetrieved.ExternalId = modelRevisionToCreate.ExternalId @>

            test <@ modelRevisionUpdated.Status = SimulatorModelRevisionStatus.failure @>
            test <@ modelRevisionUpdated.BoundaryConditionsStatus = SimulatorModelRevisionStatus.success @>
            test <@ modelRevisionUpdated.StatusMessage = "test" @>


        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore

            azureDevClient.Files.DeleteAsync([ new Identity(fileToCreate.ExternalId) ])
            |> ignore
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorRoutines")>]
let ``Create simulator routines is Ok`` () =
    task {
        // Arrange
        let routineExternalId = $"test_routine_3_{now}"
        let routineExternalIdPredefined = $"{routineExternalId}_predefined"
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
                    ExternalId = routineExternalIdPredefined,
                    ModelExternalId = modelCreated.ExternalId,
                    SimulatorIntegrationExternalId = integrationCreated.ExternalId,
                    CalculationType = "IPR/VLP"
                )

            // Act
            let! resRoutine = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resRoutinePredefined =
                azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesPredefinedAsync([ routineToCreatePredefined ])

            let! resList =
                azureDevClient.Alpha.Simulators.ListSimulatorRoutinesAsync(
                    new SimulatorRoutineQuery(
                        Filter = SimulatorRoutineFilter(ModelExternalIds = [ modelCreated.ExternalId ])
                    )
                )

            let resListRoutine =
                resList.Items |> Seq.find (fun item -> item.ExternalId = routineExternalId)

            let resListRoutinePredefined =
                resList.Items
                |> Seq.find (fun item -> item.ExternalId = routineExternalIdPredefined)

            let! resDeleteRoutine =
                azureDevClient.Alpha.Simulators.DeleteSimulatorRoutinesAsync([ new Identity(resListRoutine.Id) ])

            // Assert
            test <@ Seq.length resRoutine = 1 @>
            test <@ Seq.length resRoutinePredefined = 1 @>

            test <@ resListRoutine.Name = routineToCreate.Name @>
            test <@ resListRoutinePredefined.CalculationType = routineToCreatePredefined.CalculationType @>
            test <@ resListRoutinePredefined.Name = "Rate by Nodal Analysis" @>

            test <@ isNull resDeleteRoutine |> not @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
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

            let chokeCurve =
                JsonSerializer.SerializeToElement(
                    {| unit = "m2"
                       setting = [ 99.8 ]
                       opening = [ 10.1; 0.1 ] |}
                )


            let revisionToCreate =
                SimulatorRoutineRevisionCreate(
                    ExternalId = routineRevisionExternalId,
                    RoutineExternalId = routineExternalId,
                    Configuration =
                        SimulatorRoutineRevisionConfiguration(
                            Schedule = SimulatorRoutineRevisionSchedule(Enabled = true, StartTime = 100, Repeat = "15m"),
                            LogicalCheck =
                                SimulatorRoutineRevisionLogicalCheck(
                                    Enabled = true,
                                    TimeseriesExternalId = "test",
                                    Value = 10.9,
                                    Operator = "gt",
                                    Aggregate = "average"
                                ),
                            SteadyStateDetection =
                                SimulatorRoutineRevisionSteadyStateDetection(
                                    Enabled = true,
                                    TimeseriesExternalId = "test",
                                    Aggregate = "average",
                                    MinSectionSize = 1,
                                    VarThreshold = 0.1,
                                    SlopeThreshold = 0.1
                                ),
                            DataSampling =
                                SimulatorRoutineRevisionDataSampling(
                                    ValidationWindow = 1,
                                    SamplingWindow = 1,
                                    Granularity = 1,
                                    ValidationEndOffset = "1m"
                                ),
                            InputTimeseries = [],
                            OutputTimeseries = [],
                            OutputSequences =
                                [ SimulatorRoutineRevisionOutputSequence(Name = "test", ReferenceId = "test") ],

                            ExtraOptions = Dictionary(dict [ "chokeCurve", chokeCurve ])
                        )
                )

            // Act
            let! resRoutinePredefined =
                azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesPredefinedAsync([ routineToCreatePredefined ])

            let! resRevision =
                azureDevClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revisionToCreate ])

            let! resRevisionRetrieve =
                azureDevClient.Alpha.Simulators.RetrieveSimulatorRoutineRevisionsAsync(
                    [ new Identity(routineRevisionExternalId) ]
                )

            let retrievedRevision = resRevisionRetrieve |> Seq.head

            // Assert
            let lenPredefinedRoutine = Seq.length resRoutinePredefined
            test <@ lenPredefinedRoutine = 1 @>

            test <@ resRevision |> Seq.length = 1 @>
            let revision = resRevision |> Seq.head

            test <@ resRevisionRetrieve |> Seq.length = 1 @>
            test <@ retrievedRevision.ExternalId = routineRevisionExternalId @>

            test <@ revision.ExternalId = routineRevisionExternalId @>
            test <@ revision.RoutineExternalId = routineExternalId @>
            test <@ revision.Configuration.ToString() = revisionToCreate.Configuration.ToString() @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
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

            let scriptStage =
                SimulatorRoutineRevisionScriptStage(
                    Order = 1,
                    Description = "test",
                    Steps =
                        [ SimulatorRoutineRevisionScriptStep(
                              Order = 1,
                              StepType = "Set",
                              Description = "test",
                              Arguments =
                                  new Dictionary<string, string>(
                                      dict
                                          [ "argumentType", "inputTimeSeries"
                                            "referenceId", "test"
                                            "objectName", "test"
                                            "objectProperty", "test2" ]
                                  )
                          )
                          SimulatorRoutineRevisionScriptStep(
                              Order = 2,
                              StepType = "Get",
                              Description = "test",
                              Arguments =
                                  new Dictionary<string, string>(
                                      dict
                                          [ "argumentType", "outputTimeSeries"
                                            "referenceId", "test"
                                            "objectName", "test"
                                            "objectProperty", "test2" ]
                                  )
                          ) ]
                )

            let revisionToCreate =
                SimulatorRoutineRevisionCreate(
                    ExternalId = routineRevisionExternalId,
                    RoutineExternalId = routineExternalId,
                    Script = [ scriptStage ],
                    Configuration =
                        SimulatorRoutineRevisionConfiguration(
                            Schedule = SimulatorRoutineRevisionSchedule(Enabled = false),
                            LogicalCheck = SimulatorRoutineRevisionLogicalCheck(Enabled = false),
                            SteadyStateDetection = SimulatorRoutineRevisionSteadyStateDetection(Enabled = false),
                            DataSampling =
                                SimulatorRoutineRevisionDataSampling(
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
            let! _ = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resRevision =
                azureDevClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revisionToCreate ])

            let! resListRevisions =
                azureDevClient.Alpha.Simulators.ListSimulatorRoutineRevisionsAsync(
                    new SimulatorRoutineRevisionQuery(
                        Filter =
                            SimulatorRoutineRevisionFilter(
                                RoutineExternalIds = [ routineExternalId ],
                                SimulatorIntegrationExternalIds = [ integrationCreated.ExternalId ],
                                SimulatorExternalIds = [ simulatorExternalId ],
                                CreatedTime = TimeRange(Min = now - 10000L)
                            )
                    )
                )

            let revisionFound =
                resListRevisions.Items
                |> Seq.find (fun item -> item.ExternalId = routineRevisionExternalId)

            // Assert
            test <@ resRevision |> Seq.length = 1 @>

            test <@ revisionFound.ExternalId = routineRevisionExternalId @>
            test <@ revisionFound.RoutineExternalId = routineExternalId @>
            test <@ revisionFound.Configuration.ToString() = revisionToCreate.Configuration.ToString() @>

            let scriptStageRes = Seq.head revisionFound.Script
            test <@ scriptStageRes.ToString() = scriptStage.ToString() @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorLogs")>]
let ``Update simulation log is Ok`` () =
    task {
        // Arrange
        let! listRunsRes =
            azureDevClient.Alpha.Simulators.ListSimulationRunsAsync(
                new SimulationRunQuery(
                    Sort = [ new SimulatorSortItem(Property = "createdTime", Order = SimulatorSortOrder.desc) ]
                )
            )

        let firstRunWithLogId =
            listRunsRes.Items |> Seq.find (fun item -> item.LogId.HasValue)

        let logId = firstRunWithLogId.LogId.Value
        let logEntryStr = $"test log {DateTimeOffset.Now.ToUnixTimeMilliseconds()}"

        let simulatorLogUpdateData =
            SimulatorLogDataEntry(
                Message = logEntryStr,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Severity = "Information"
            )

        let simulatorLogUpdateItem =
            SimulatorLogUpdateItem(
                id = logId,
                Update =
                    SimulatorLogUpdate(Data = UpdateEnumerable<SimulatorLogDataEntry>([ simulatorLogUpdateData ], null))
            )

        // Act
        let! _ = azureDevClient.Alpha.Simulators.UpdateSimulatorLogsAsync([ simulatorLogUpdateItem ])
        let! retrieveLogRes = azureDevClient.Alpha.Simulators.RetrieveSimulatorLogsAsync([ new Identity(logId) ])

        // Assert
        test <@ Seq.length retrieveLogRes = 1 @>
        let logEntry = retrieveLogRes |> Seq.head
        test <@ logEntry.Data |> Seq.length >= 1 @>
        let lastLogEntryData = logEntry.Data |> Seq.last
        test <@ lastLogEntryData.Message = simulatorLogUpdateData.Message @>
    }
