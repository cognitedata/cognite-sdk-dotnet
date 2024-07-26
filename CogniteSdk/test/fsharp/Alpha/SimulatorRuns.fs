module Tests.Integration.SimulatorRuns

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open CogniteSdk.Alpha
open CogniteSdk
open Common


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
        let listQuery = SimulationRunQuery()

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
