module Tests.Integration.Simulators

open System

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Alpha
open Tests.Integration.Alpha.Common
open System.Collections.Generic

let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
let simulatorExternalId = $"test_sim_{now}"

// Helper function to compare two sequences of items considering their string representations
let (====) (seq1: IEnumerable<'T>) (seq2: IEnumerable<'T>) =
    List.ofSeq (seq1 |> Seq.map string) = List.ofSeq (seq2 |> Seq.map string)

[<Fact>]
[<Trait("resource", "simulators")>]
[<Trait("api", "simulators")>]
let ``Create, update and delete simulators is Ok`` () =
    task {

        let fileExtensionTypes = seq { "json" }

        let modelDependencies =
            seq {
                SimulatorModelDependency(
                    FileExtensionTypes = seq { "json" },
                    Fields = [ SimulatorModelDependencyField(Name = "cell", Label = "Cell", Info = "Cell number") ]
                )

            }
        // Arrange & act
        let itemToCreate = testSimulatorCreate simulatorExternalId
        let! itemCreatedRes = writeClient.Alpha.Simulators.CreateAsync [ itemToCreate ]
        let itemCreated = itemCreatedRes |> Seq.head

        let itemToUpdate =
            new SimulatorUpdateItem(
                id = itemCreated.Id,
                Update =
                    SimulatorUpdate(
                        Name = Update<string>(itemCreated.Name + "_upd"),
                        FileExtensionTypes = new Update<IEnumerable<string>>(itemCreated.FileExtensionTypes),
                        ModelTypes = Update<IEnumerable<SimulatorModelType>> itemCreated.ModelTypes,
                        ModelDependencies = Update<IEnumerable<SimulatorModelDependency>> modelDependencies,
                        StepFields = Update<IEnumerable<SimulatorStepField>> itemCreated.StepFields,
                        UnitQuantities = Update<IEnumerable<SimulatorUnitQuantity>> itemCreated.UnitQuantities
                    )
            )

        let! itemUpdateRes = writeClient.Alpha.Simulators.UpdateAsync [ itemToUpdate ]
        let! _ = writeClient.Alpha.Simulators.DeleteAsync [ new Identity(itemToCreate.ExternalId) ]

        // Assert
        let len = Seq.length itemCreatedRes
        test <@ len = 1 @>

        test <@ itemCreated.Name = itemToCreate.Name @>
        test <@ itemCreated.FileExtensionTypes ==== fileExtensionTypes @>
        test <@ itemCreated.CreatedTime >= now @>
        test <@ itemCreated.LastUpdatedTime >= now @>

        let itemUpdated = itemUpdateRes |> Seq.head
        test <@ itemUpdated.Id = itemCreated.Id @>
        test <@ itemUpdated.Name = itemToUpdate.Update.Name.Set @>

        test <@ itemUpdated.ModelTypes ==== itemCreated.ModelTypes @>
        test <@ itemUpdated.ModelDependencies ==== modelDependencies @>
        test <@ itemUpdated.FileExtensionTypes ==== itemCreated.FileExtensionTypes @>
        test <@ itemUpdated.StepFields ==== itemCreated.StepFields @>
        test <@ itemUpdated.UnitQuantities ==== itemCreated.UnitQuantities @>

        test <@ itemUpdated.CreatedTime = itemCreated.CreatedTime @>
        test <@ itemUpdated.LastUpdatedTime > itemCreated.LastUpdatedTime @>
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
[<Trait("resource", "simulatorIntegrations")>]
[<Trait("api", "simulators")>]
let ``Create and update simulator integration is Ok`` () =
    task {
        // Arrange
        let simulatorExternalId = $"test_sim_2_{now}"
        let integrationExternalId = $"test_integration_{now}"

        let simulatorToCreate = testSimulatorCreate (simulatorExternalId)

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
[<Trait("resource", "simulatorIntegrations")>]
[<Trait("api", "simulators")>]
let ``List simulator integrations is Ok`` () =
    task {
        // Arrange
        let query =
            SimulatorIntegrationQuery(Filter = SimulatorIntegrationFilter(Active = true))

        // Act
        let! res = writeClient.Alpha.Simulators.ListSimulatorIntegrationsAsync(query)

        let len = Seq.length res.Items

        // Assert
        test <@ len >= 0 @>
    }
