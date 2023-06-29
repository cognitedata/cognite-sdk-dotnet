module Tests.Integration.Simulators

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

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
[<Trait("resource", "simulators")>]
let ``Create simulation runs is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()

        let itemToCreate =
            SimulationRunCreate(
                SimulatorName = "DWSIM",
                ModelName = "ShowerMixerIntegrationTest",
                RoutineName = "ShowerMixerCalculation",
                RunType = SimulationRunType.external
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
        test <@ now - itemRes.CreatedTime < 10000 @>
        test <@ now - itemRes.LastUpdatedTime < 10000 @>

    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
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
        test <@ res.Items |> Seq.forall (fun item -> item.EventId = Nullable() ) @>

        test
            <@
                res.Items
                |> Seq.forall (fun item -> item.CreatedTime > 0 && item.LastUpdatedTime > 0)
            @>

        // Assert
        test <@ len > 0 @>
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
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
