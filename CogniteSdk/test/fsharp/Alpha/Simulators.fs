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

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
let ``Create and delete simulators is Ok`` () =
    task {

        // Arrange
        let itemToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [| "json" |],
                Enabled = true
            )

        // Act
        let! res = azureDevClient.Alpha.Simulators.CreateAsync([ itemToCreate ])

        // Assert
        let len = Seq.length res
        test <@ len = 1 @>
        let itemRes = res |> Seq.head

        test <@ itemRes.Name = itemToCreate.Name @>
        test <@ itemRes.Enabled = itemToCreate.Enabled.Value @>
        test <@ itemRes.FileExtensionTypes = itemToCreate.FileExtensionTypes @>
        test <@ itemRes.CreatedTime >= now @>
        test <@ itemRes.LastUpdatedTime >= now @>

        let! _ = azureDevClient.Alpha.Simulators.DeleteAsync [ new Identity(itemToCreate.ExternalId) ]
        ()
    }

[<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
[<Trait("resource", "simulators")>]
let ``List simulators is Ok`` () =
    task {

        // Arrange
        let query = SimulatorQuery(Filter = SimulatorFilter(Enabled = true))

        // Act
        let! res = azureDevClient.Alpha.Simulators.ListAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len > 0 @>

        test <@ res.Items |> Seq.forall (fun item -> item.Enabled = true) @>
        test <@ res.Items |> Seq.forall (fun item -> item.Name <> null) @>
    }

[<Trait("resource", "simulators")>]
let ``Create and update simulator integration is Ok`` () =
    task {
        // Arrange
        let simulatorExternalId = $"test_sim_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let simulatorToCreate =
            SimulatorCreate(
                ExternalId = simulatorExternalId,
                Name = "test_sim",
                FileExtensionTypes = [| "json" |],
                Enabled = true
            )

        let integrationToCreate =
            SimulatorIntegrationCreate(
                ExternalId = integrationExternalId,
                SimulatorExternalId = simulatorExternalId,
                DataSetId = 123,
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
