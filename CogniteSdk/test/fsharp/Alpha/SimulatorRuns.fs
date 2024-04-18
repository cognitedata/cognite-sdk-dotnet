module Tests.Integration.SimulatorRuns

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open CogniteSdk.Alpha
open Tests.Integration.Alpha.Common
open CogniteSdk


[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulationRuns")>]
let ``Create simulation runs by routine with data and callback is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()

        let itemToCreate =
            SimulationRunCreate(
                RoutineExternalId = "ShowerMixerIntegrationTestWithExtendedIO",
                RunType = SimulationRunType.external,
                Inputs =
                    [ SimulationInputOverride(
                          ReferenceId = "CWTC",
                          Value = SimulatorValue.Create(50.1), // original value is 10 C
                          Unit = SimulationInputUnitOverride(Name = "F")
                      ) ],
                ValidationEndTime = now,
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
        let! res = azureDevClient.Alpha.Simulators.CreateSimulationRunsAsync([ itemToCreate ])
        let simulationRun = res |> Seq.head

        let! resData = azureDevClient.Alpha.Simulators.ListSimulationRunsDataAsync([ simulationRun.Id ])

        callbackQuery.Id <- simulationRun.Id
        let! callbackRes = azureDevClient.Alpha.Simulators.SimulationRunCallbackAsync callbackQuery
        let simulationRunAfterCallback = callbackRes.Items |> Seq.head

        let! resDataAfterCallback = azureDevClient.Alpha.Simulators.ListSimulationRunsDataAsync([ simulationRun.Id ])


        // Assert
        let len = Seq.length res
        test <@ len = 1 @>
        let itemRes = res |> Seq.head

        test <@ itemRes.RoutineExternalId = itemToCreate.RoutineExternalId @>
        test <@ itemRes.Status = SimulationRunStatus.ready @>
        test <@ itemRes.RunType = SimulationRunType.external @>
        test <@ itemRes.ValidationEndTime = Nullable(now) @>
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
    }

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
        let! res = azureDevClient.Alpha.Simulators.ListSimulationRunsAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len = 0 @>
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
