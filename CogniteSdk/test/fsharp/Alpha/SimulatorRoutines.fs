module Tests.Integration.SimulatorRoutines

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open CogniteSdk
open CogniteSdk.Alpha
open Tests.Integration.Alpha.Common
open Common


[<Fact>]
[<Trait("resource", "simulatorRoutines")>]
let ``Create simulator routines is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        let routineExternalId = $"test_routine_3_{now}"
        let modelExternalId = $"test_model_{now}"
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            testSimulatorCreate(simulatorExternalId)

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
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        try
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! integrationCreateRes =
                writeClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])

            let integrationCreated = integrationCreateRes |> Seq.head

            let! modelCreateRes = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let modelCreated = modelCreateRes |> Seq.head

            let routineToCreate =
                SimulatorRoutineCreateCommandItem(
                    ExternalId = routineExternalId,
                    ModelExternalId = modelCreated.ExternalId,
                    SimulatorIntegrationExternalId = integrationCreated.ExternalId,
                    Name = "Test"
                )

            // Act
            let! resRoutine = writeClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resList =
                writeClient.Alpha.Simulators.ListSimulatorRoutinesAsync(
                    new SimulatorRoutineQuery(
                        Filter = SimulatorRoutineFilter(ModelExternalIds = [ modelCreated.ExternalId ])
                    )
                )

            let resListRoutine =
                resList.Items |> Seq.find (fun item -> item.ExternalId = routineExternalId)

            let! resDeleteRoutine =
                writeClient.Alpha.Simulators.DeleteSimulatorRoutinesAsync([ new Identity(resListRoutine.Id) ])

            // Assert
            test <@ Seq.length resRoutine = 1 @>

            test <@ resListRoutine.Name = routineToCreate.Name @>

            test <@ isNull resDeleteRoutine |> not @>
        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<Fact>]
[<Trait("resource", "simulatorRoutines")>]
let ``Create simulator routine revision is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        let routineExternalId = $"test_routine_3_{now}"
        let routineRevisionExternalId = $"{routineExternalId}_1"
        let modelExternalId = $"test_model_{now}"
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let simulatorToCreate =
            testSimulatorCreate(simulatorExternalId)

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
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        try
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

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
                                      dict [ "referenceId", "test"; "address", "test2" ]
                                  )
                          )
                          SimulatorRoutineRevisionScriptStep(
                              Order = 2,
                              StepType = "Get",
                              Description = "test",
                              Arguments =
                                  new Dictionary<string, string>(
                                      dict [ "referenceId", "test"; "address", "test2" ]
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
                        [
                            SimulatorRoutineRevisionInput(
                              Name = "test_input",
                              ReferenceId = "test_input",
                              Value = SimulatorValue.Create(1.0),
                              ValueType = SimulatorValueType.DOUBLE,
                              Unit = SimulatorValueUnit(Name = "test_unit", Quantity = "test_quantity")
                            )
                            SimulatorRoutineRevisionInput(
                              Name = "test_input_array",
                              ReferenceId = "test_input_array",
                              Value = SimulatorValue.Create(seq [ 1.0; 2; 3 ]),
                              ValueType = SimulatorValueType.DOUBLE_ARRAY,
                              Unit = SimulatorValueUnit(Name = "test_unit", Quantity = "test_quantity")
                            )
                        ],
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
            let! _ = writeClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let! resRevision = writeClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revToCreate ])

            let! resListRevisions =
                writeClient.Alpha.Simulators.RetrieveSimulatorRoutineRevisionsAsync(
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
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }
