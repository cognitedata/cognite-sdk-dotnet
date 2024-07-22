module Tests.Integration.Simulators

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Alpha
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
            testSimulatorCreate(simulatorExternalId)
        // Act
        let! res = writeClient.Alpha.Simulators.CreateAsync([ itemToCreate ])
        let! _ = writeClient.Alpha.Simulators.DeleteAsync [ new Identity(itemToCreate.ExternalId) ]

        // Assert
        let len = Seq.length res
        test <@ len = 1 @>
        let itemRes = res |> Seq.head

        test <@ itemRes.Name = itemToCreate.Name @>
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
        let query = SimulatorQuery(Filter = SimulatorFilter())

        // Act
        let! res = writeClient.Alpha.Simulators.ListAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len > 0 @>

        test <@ res.Items |> Seq.forall (fun item -> item.Name <> null) @>
    }

[<Fact>]
[<Trait("resource", "simulators")>]
let ``Create and update simulator integration is Ok`` () =
    task {
        // Arrange
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let simulatorToCreate = testSimulatorCreate(simulatorExternalId)

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
                            ConnectorStatus = Update<string>("RUNNING_SIMULATION"),
                            SimulatorVersion = Update<string>("2.3.4"),
                            LicenseStatus = Update<string>("DISABLED"),
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

            test <@ integrationUpdated.ConnectorStatus = "RUNNING_SIMULATION" @>
            test <@ integrationUpdated.SimulatorVersion = "2.3.4" @>
            test <@ integrationUpdated.ConnectorStatusUpdatedTime = Nullable now @>
            test <@ integrationUpdated.LicenseLastCheckedTime = Nullable now @>
        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
    }

[<Fact>]
// [<FactIf(envVar = "ENABLE_SIMULATORS_TESTS", skipReason = "Immature Simulator APIs")>]
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
