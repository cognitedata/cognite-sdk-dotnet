module Tests.Integration.SimulatorRoutines

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open CogniteSdk
open CogniteSdk.Alpha
open System.Text.Json
open Tests.Integration.Alpha.Common

let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()


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
[<Trait("resource", "simulatorRoutines")>]
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
            
            test <@ revision.Configuration.Schedule.ToString() = revisionToCreate.Configuration.Schedule.ToString() @>
            test <@ revision.Configuration.LogicalCheck.ToString() = revisionToCreate.Configuration.LogicalCheck.ToString() @>
            test <@ revision.Configuration.SteadyStateDetection.ToString() = revisionToCreate.Configuration.SteadyStateDetection.ToString() @>
            test <@ revision.Configuration.DataSampling.ToString() = revisionToCreate.Configuration.DataSampling.ToString() @>
            
            test <@ revision.Configuration.InputTimeseries |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revisionToCreate.Configuration.InputTimeseries @>
            test <@ revision.Configuration.OutputTimeseries |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revisionToCreate.Configuration.OutputTimeseries @>

        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorRoutines")>]
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

            let! resRetrieveRevisions =
                azureDevClient.Alpha.Simulators.RetrieveSimulatorRoutineRevisionsAsync(
                    [ new Identity(routineRevisionExternalId) ]
                )

            let revision = resRetrieveRevisions |> Seq.head
            // Assert
            test <@ resRevision |> Seq.length = 1 @>

            test <@ revision.ExternalId = routineRevisionExternalId @>
            test <@ revision.RoutineExternalId = routineExternalId @>

            let revConfig = revision.Configuration
            let revConfigToCreate = revisionToCreate.Configuration

            test <@ revConfig.DataSampling.ToString() = revConfigToCreate.DataSampling.ToString() @>
            test <@ revConfig.Schedule.ToString() = revConfigToCreate.Schedule.ToString() @>
            test <@ revConfig.LogicalCheck.ToString() = revConfigToCreate.LogicalCheck.ToString() @>
            test <@ revConfig.SteadyStateDetection.ToString() = revConfigToCreate.SteadyStateDetection.ToString() @>

            test <@ revConfig.InputTimeseries |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revConfigToCreate.InputTimeseries @>
            test <@ revConfig.OutputTimeseries |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revConfigToCreate.OutputTimeseries @>
            test <@ revConfig.InputConstants |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revConfigToCreate.InputConstants @>

            let scriptStageRes = Seq.head revision.Script
            test <@ scriptStageRes.ToString() = scriptStage.ToString() @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorRoutines")>]
let ``Create simulator routine revisions with extended inputs / outputs is Ok`` () =
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

            let! _ = azureDevClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])

            let! _ = azureDevClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let routineToCreate =
                SimulatorRoutineCreateCommandItem(
                    ExternalId = routineExternalId,
                    ModelExternalId = modelToCreate.ExternalId,
                    SimulatorIntegrationExternalId = integrationToCreate.ExternalId,
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

            let revisionConfigurationToCreate = SimulatorRoutineRevisionConfiguration(
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
                Inputs = [
                    SimulatorRoutineRevisionInput(
                        Name = "test_input",
                        ReferenceId = "test_input",
                        Value = SimulatorValue.Create(1.0),
                        ValueType = SimulatorValueType.DOUBLE,
                        Unit = SimulatorValueUnit(
                            Name = "test_unit",
                            Type = "test_type"
                        )
                    )
                ],
                Outputs = [ SimulatorRoutineRevisionOutput(Name = "test", ReferenceId = "test") ]
            )

            let revisionToCreate =
                SimulatorRoutineRevisionCreate(
                    ExternalId = routineRevisionExternalId,
                    RoutineExternalId = routineExternalId,
                    Script = [ scriptStage ],
                    Configuration = revisionConfigurationToCreate
                )

            // Act
            let! _ = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resRevision =
                azureDevClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revisionToCreate ])

            let! resListRevisions =
                azureDevClient.Alpha.Simulators.RetrieveSimulatorRoutineRevisionsAsync(
                    [ new Identity(routineRevisionExternalId) ]
                )

            let revision = resListRevisions |> Seq.head
            let revConfig = revision.Configuration
            let revConfigToCreate = revisionToCreate.Configuration

            // Assert
            test <@ resRevision |> Seq.length = 1 @>

            test <@ revision.ExternalId = routineRevisionExternalId @>
            test <@ revision.RoutineExternalId = routineExternalId @>

            test <@ revConfig.DataSampling.ToString() = revConfigToCreate.DataSampling.ToString() @>
            test <@ revConfig.Schedule.ToString() = revConfigToCreate.Schedule.ToString() @>
            test <@ revConfig.LogicalCheck.ToString() = revConfigToCreate.LogicalCheck.ToString() @>
            test <@ revConfig.SteadyStateDetection.ToString() = revConfigToCreate.SteadyStateDetection.ToString() @>

            test <@ revConfig.Inputs |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revConfigToCreate.Inputs @>
            test <@ revConfig.Outputs |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revConfigToCreate.Outputs @>

            let scriptStageRes = Seq.head revision.Script
            test <@ scriptStageRes.ToString() = scriptStage.ToString() @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

