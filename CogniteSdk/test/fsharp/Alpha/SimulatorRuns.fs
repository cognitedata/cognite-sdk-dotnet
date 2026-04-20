module Tests.Integration.SimulatorRuns

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open CogniteSdk.Alpha
open CogniteSdk
open Common
open Tests.Integration.Alpha.Common


[<Fact>]
[<Trait("resource", "simulationRuns")>]
[<Trait("api", "simulators")>]
let ``Create simulation runs by routine with data and callback is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()

        let itemToCreate =
            SimulationRunCreate(
                RoutineExternalId = "ShowerMixerForTests",
                RunType = SimulationRunType.external,
                Inputs =
                    [ SimulationInputOverride(
                          ReferenceId = "CWTC",
                          Value = SimulatorValue.Create(50.1), // original value is 10 C
                          Unit = SimulationInputUnitOverride(Name = "F")
                      ) ],
                LogSeverity = "Debug",
                RunTime = now,
                Queue = true
            )

        let callbackQuery =
            SimulationRunCallbackItem(
                Status = SimulationRunStatus.success,
                StatusMessage = "Artificially set success via integration test",
                Outputs =
                    [ SimulatorValueItem(
                          ReferenceId = "ST",
                          Value = SimulatorValue.Create(100),
                          ValueType = SimulatorValueType.DOUBLE,
                          Unit = SimulatorValueUnit(Name = "F")
                      ) ]
            )

        // Act
        let! res = writeClient.Alpha.Simulators.CreateSimulationRunsAsync([ itemToCreate ])
        let simulationRun = res |> Seq.head

        let! resData = writeClient.Alpha.Simulators.ListSimulationRunsDataAsync([ simulationRun.Id ])

        callbackQuery.Id <- simulationRun.Id
        let! callbackRes = writeClient.Alpha.Simulators.SimulationRunCallbackAsync callbackQuery
        let simulationRunAfterCallback = callbackRes.Items |> Seq.head

        let! resDataAfterCallback = writeClient.Alpha.Simulators.ListSimulationRunsDataAsync([ simulationRun.Id ])

        let! resSimRunLogs = writeClient.Alpha.Simulators.RetrieveSimulatorLogsAsync(
            [ new Identity(simulationRun.LogId.Value) ]
        )


        // Assert
        let len = Seq.length res
        test <@ len = 1 @>
        let itemRes = res |> Seq.head

        test <@ itemRes.RoutineExternalId = itemToCreate.RoutineExternalId @>
        test <@ itemRes.Status = SimulationRunStatus.ready @>
        test <@ itemRes.RunType = SimulationRunType.external @>
        test <@ itemRes.RunTime = Nullable(now) @>
        test <@ now - itemRes.CreatedTime < 10000 @>
        test <@ now - itemRes.LastUpdatedTime < 10000 @>

        // Assert simulation data
        let data = resData |> Seq.head
        let item = data.Inputs |> Seq.tryFind (fun x -> x.ReferenceId = "CWTC")
        test <@ item.IsSome @>
        let item = item.Value
        test <@ item.Value = SimulatorValue.Create(50.1) @>
        test <@ item.Unit.Name = "F" @>

        // Assert callback data
        test <@ Seq.length callbackRes.Items = 1 @>

        test <@ simulationRunAfterCallback.Status = SimulationRunStatus.success @>

        let dataAfterCallback = resDataAfterCallback |> Seq.head
        let item = dataAfterCallback.Outputs |> Seq.tryFind (fun x -> x.ReferenceId = "ST")
        test <@ item.IsSome @>
        let item = item.Value
        test <@ item.Value = SimulatorValue.Create(100) @>
        test <@ item.Unit.Name = "F" @>

        // Assert log 
        test <@ resSimRunLogs |> Seq.length = 1 @>
        let logItem = resSimRunLogs |> Seq.head
        test <@ logItem.Severity = "Debug" @>
    }

[<Fact>]
[<Trait("resource", "simulationRuns")>]
[<Trait("api", "simulators")>]
let ``List simulation runs is Ok`` () =
    task {

        // Arrange
        let query =
            SimulationRunQuery(
                Filter = SimulationRunFilter(SimulatorExternalIds = [ "DWSIM" ], Status = SimulationRunStatus.success)
            )

        // Act
        let! res = writeClient.Alpha.Simulators.ListSimulationRunsAsync(query)

        let len = Seq.length res.Items

        test <@ res.Items |> Seq.forall (fun item -> item.SimulatorExternalId = "DWSIM") @>
        test <@ res.Items |> Seq.forall (fun item -> item.Status = SimulationRunStatus.success) @>

        test
            <@
                res.Items
                |> Seq.forall (fun item -> item.CreatedTime > 0 && item.LastUpdatedTime > 0)
            @>

        // Assert
        test <@ len > 0 @>
    }

[<Fact>]
[<Trait("resource", "simulationRuns")>]
[<Trait("api", "simulators")>]
let ``List simulation runs with external id filters is Ok`` () =
    task {

        // Arrange
        let query =
            SimulationRunQuery(
                Filter =
                    SimulationRunFilter(
                        SimulatorExternalIds = [ "do_not_exist" ],
                        ModelRevisionExternalIds = [ "do_not_exist" ],
                        RoutineRevisionExternalIds = [ "do_not_exist" ]
                    )
            )

        // Act
        let! res = writeClient.Alpha.Simulators.ListSimulationRunsAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len = 0 @>
    }

[<Fact>]
[<Trait("resource", "simulationRuns")>]
[<Trait("api", "simulators")>]
let ``Retrieve simulation runs is Ok`` () =
    task {

        // Arrange
        let listQuery: SimulationRunQuery = SimulationRunQuery()

        let! listRes = writeClient.Alpha.Simulators.ListSimulationRunsAsync(listQuery)

        test <@ Seq.length listRes.Items > 0 @>

        let simulationRun = listRes.Items |> Seq.head

        // Act
        let! res = writeClient.Alpha.Simulators.RetrieveSimulationRunsAsync [ simulationRun.Id ]
        let simulationRunRetrieveRes = res |> Seq.head

        // Assert
        test <@ Seq.length res = 1 @>
        test <@ simulationRunRetrieveRes.Status = SimulationRunStatus.success @>
    }


[<Fact>]
[<Trait("resource", "simulationRuns")>]
[<Trait("api", "simulators")>]
let ``Retrieve simulation runs with cursor is Ok`` () =
    task {

        // Arrange
        let listQuery: SimulationRunQuery = SimulationRunQuery(Limit = 1)

        let! (listRes: IItemsWithCursor<SimulationRun>) = writeClient.Alpha.Simulators.ListSimulationRunsAsync(listQuery)

        test <@ Seq.length listRes.Items > 0 @>
        test <@ isNull listRes.NextCursor |> not @>
    }

[<Fact>]
[<Trait("resource", "simulatorLogs")>]
[<Trait("api", "simulators")>]
let ``Update simulation log is Ok`` () =
    task {
        // Arrange
        let! listRunsRes =
            writeClient.Alpha.Simulators.ListSimulationRunsAsync(
                new SimulationRunQuery(
                    Filter = new SimulationRunFilter(RoutineRevisionExternalIds = [ "ShowerMixerForTests-1" ]),
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
        let! _ = writeClient.Alpha.Simulators.UpdateSimulatorLogsAsync([ simulatorLogUpdateItem ])
        let! retrieveLogRes = writeClient.Alpha.Simulators.RetrieveSimulatorLogsAsync([ new Identity(logId) ])

        // Assert
        test <@ Seq.length retrieveLogRes = 1 @>
        let logEntry = retrieveLogRes |> Seq.head
        test <@ logEntry.Data |> Seq.length >= 1 @>
        let lastLogEntryData = logEntry.Data |> Seq.last
        test <@ lastLogEntryData.Message = simulatorLogUpdateData.Message @>
    }

[<Fact>]
[<Trait("resource", "simulationRuns")>]
[<Trait("api", "simulators")>]
let ``Poll simulation runs assigns queued run to connector is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        let simulatorExternalId = $"test_sim_poll_{now}"
        let integrationExternalId = $"test_integration_poll_{now}"
        let modelExternalId = $"test_model_poll_{now}"
        let routineExternalId = $"test_routine_poll_{now}"
        let routineRevisionExternalId = $"{routineExternalId}_1"

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

        let modelToCreate =
            SimulatorModelCreate(
                ExternalId = modelExternalId,
                SimulatorExternalId = simulatorExternalId,
                Name = "test_model_poll",
                Description = "test model for poll test",
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        try
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ testSimulatorCreate simulatorExternalId ])
            let! _ = writeClient.Alpha.Simulators.CreateSimulatorIntegrationAsync([ integrationToCreate ])
            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])

            let! testFileRes = writeClient.Files.RetrieveAsync([ new Identity("empty.json") ])
            let testFileId = testFileRes |> Seq.head |> (fun f -> f.Id)

            let modelRevisionToCreate =
                SimulatorModelRevisionCreate(
                    ExternalId = $"test_model_revision_poll_{now}",
                    ModelExternalId = modelExternalId,
                    Description = "test model revision for poll test",
                    FileId = testFileId
                )

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate ])

            let routineToCreate =
                SimulatorRoutineCreateCommandItem(
                    ExternalId = routineExternalId,
                    ModelExternalId = modelExternalId,
                    Name = "Test Poll Routine"
                )

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorRoutinesAsync([ routineToCreate ])

            let revisionToCreate =
                SimulatorRoutineRevisionCreate(
                    ExternalId = routineRevisionExternalId,
                    RoutineExternalId = routineExternalId,
                    Script =
                        [ SimulatorRoutineRevisionScriptStage(
                              Order = 1,
                              Description = "test",
                              Steps =
                                  [ SimulatorRoutineRevisionScriptStep(
                                        Order = 1,
                                        StepType = "Get",
                                        Description = "test",
                                        Arguments =
                                            new System.Collections.Generic.Dictionary<string, string>(
                                                dict [ "referenceId", "test"; "address", "test" ]
                                            )
                                    ) ]
                          ) ],
                    Configuration =
                        SimulatorRoutineRevisionConfiguration(
                            Schedule = SimulatorRoutineRevisionSchedule(Enabled = false),
                            LogicalCheck = [],
                            SteadyStateDetection = [],
                            DataSampling =
                                SimulatorRoutineRevisionDataSampling(
                                    Enabled = true,
                                    SamplingWindow = 1,
                                    Granularity = 1
                                ),
                            Inputs = [],
                            Outputs = [ SimulatorRoutineRevisionOutput(Name = "test", ReferenceId = "test") ]
                        )
                )

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorRoutineRevisionsAsync([ revisionToCreate ])

            let! createRes =
                writeClient.Alpha.Simulators.CreateSimulationRunsAsync(
                    [ SimulationRunCreate(
                          RoutineExternalId = routineExternalId,
                          RunType = SimulationRunType.external,
                          RunTime = now,
                          Queue = true
                      ) ]
                )

            let simulationRun = createRes |> Seq.head

            let pollItem =
                SimulationRunPollItem(SimulatorIntegrationExternalId = integrationExternalId, Limit = 10)

            try
                let! pollRes = writeClient.Alpha.Simulators.PollSimulationRunsAsync([ pollItem ])

                // Assert
                test <@ simulationRun.Status = SimulationRunStatus.queued @>
                let assignedRun = pollRes.Items |> Seq.tryFind (fun r -> r.Id = simulationRun.Id)
                test <@ assignedRun.IsSome @>

                let assigned = assignedRun.Value
                test <@ assigned.Status = SimulationRunStatus.ready @>
                test <@ assigned.SimulatorIntegrationExternalId = integrationExternalId @>

                let! _ =
                    writeClient.Alpha.Simulators.SimulationRunCallbackAsync(
                        SimulationRunCallbackItem(
                            Id = simulationRun.Id,
                            Status = SimulationRunStatus.failure,
                            StatusMessage = "Integration test cleanup",
                            SimulatorIntegrationExternalId = integrationExternalId
                        )
                    )

                ()
            with :? CogniteSdk.ResponseException as ex when ex.Code = 409 ->
                ()
        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ]).GetAwaiter().GetResult()
            |> ignore
    }
