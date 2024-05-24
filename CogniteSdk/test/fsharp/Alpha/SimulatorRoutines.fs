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
        let modelExternalId = $"test_model_{now}"
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let! dataSetRes = azureDevClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            SimulatorCreate(ExternalId = simulatorExternalId, Name = "test_sim", FileExtensionTypes = [ "json" ])

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = dataSet.Id,
                SimulatorVersion = "N/A",
                ConnectorVersion = "1.2.3"
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

            // Act
            let! resRoutine = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resList =
                azureDevClient.Alpha.Simulators.ListSimulatorRoutinesAsync(
                    new SimulatorRoutineQuery(
                        Filter = SimulatorRoutineFilter(ModelExternalIds = [ modelCreated.ExternalId ])
                    )
                )

            let resListRoutine =
                resList.Items |> Seq.find (fun item -> item.ExternalId = routineExternalId)

            let! resDeleteRoutine =
                azureDevClient.Alpha.Simulators.DeleteSimulatorRoutinesAsync([ new Identity(resListRoutine.Id) ])

            // Assert
            test <@ Seq.length resRoutine = 1 @>

            test <@ resListRoutine.Name = routineToCreate.Name @>

            test <@ isNull resDeleteRoutine |> not @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulatorRoutines")>]
let ``Create simulator routine revision is Ok`` () =
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
            SimulatorCreate(ExternalId = simulatorExternalId, Name = "test_sim", FileExtensionTypes = [ "json" ])

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = dataSet.Id,
                SimulatorVersion = "N/A",
                ConnectorVersion = "1.2.3"
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
                                      dict [ "referenceId", "test"; "objectName", "test"; "objectProperty", "test2" ]
                                  )
                          )
                          SimulatorRoutineRevisionScriptStep(
                              Order = 2,
                              StepType = "Get",
                              Description = "test",
                              Arguments =
                                  new Dictionary<string, string>(
                                      dict [ "referenceId", "test"; "objectName", "test"; "objectProperty", "test2" ]
                                  )
                          ) ]
                )

            let revisionConfigurationToCreate =
                SimulatorRoutineRevisionConfiguration(
                    Schedule = SimulatorRoutineRevisionSchedule(Enabled = false),
                    LogicalCheck = [],
                    SteadyStateDetection = [],
                    DataSampling =
                        SimulatorRoutineRevisionDataSampling(
                            Enabled = true,
                            ValidationWindow = 1,
                            SamplingWindow = 1,
                            Granularity = 1
                        ),
                    Inputs =
                        [ SimulatorRoutineRevisionInput(
                              Name = "test_input",
                              ReferenceId = "test_input",
                              Value = SimulatorValue.Create(1.0),
                              ValueType = SimulatorValueType.DOUBLE,
                              Unit = SimulatorValueUnit(Name = "test_unit", Quantity = "test_quantity")
                          ) ],
                    Outputs = [ SimulatorRoutineRevisionOutput(Name = "test", ReferenceId = "test") ]
                )

            let revToCreate =
                SimulatorRoutineRevisionCreate(
                    ExternalId = routineRevisionExternalId,
                    RoutineExternalId = routineExternalId,
                    Script = [ scriptStage ],
                    Configuration = revisionConfigurationToCreate
                )

            // Act
            let! _ = azureDevClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resRevision = azureDevClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revToCreate ])

            let! resListRevisions =
                azureDevClient.Alpha.Simulators.RetrieveSimulatorRoutineRevisionsAsync(
                    [ new Identity(routineRevisionExternalId) ]
                )

            let revision = resListRevisions |> Seq.head
            let revConfig = revision.Configuration
            let revConfigToCreate = revToCreate.Configuration

            // Assert
            test <@ resRevision |> Seq.length = 1 @>

            test <@ revision.ExternalId = routineRevisionExternalId @>
            test <@ revision.RoutineExternalId = routineExternalId @>

            test <@ revConfig.DataSampling.ToString() = revConfigToCreate.DataSampling.ToString() @>
            test <@ revConfig.Schedule.ToString() = revConfigToCreate.Schedule.ToString() @>

            test
                <@
                    revConfig.LogicalCheck
                    |> Seq.forall2
                        (fun (a: SimulatorRoutineRevisionLogicalCheck) b -> a.ToString() = b.ToString())
                        revConfigToCreate.LogicalCheck
                @>

            test
                <@
                    revConfig.SteadyStateDetection
                    |> Seq.forall2
                        (fun (a: SimulatorRoutineRevisionSteadyStateDetection) b -> a.ToString() = b.ToString())
                        revConfigToCreate.SteadyStateDetection
                @>

            test
                <@
                    revConfig.Inputs
                    |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revConfigToCreate.Inputs
                @>

            test
                <@
                    revConfig.Outputs
                    |> Seq.forall2 (fun a b -> a.ToString() = b.ToString()) revConfigToCreate.Outputs
                @>

            let scriptStageRes = Seq.head revision.Script
            test <@ scriptStageRes.ToString() = scriptStage.ToString() @>
        finally
            azureDevClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }
