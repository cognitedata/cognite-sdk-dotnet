module Tests.Integration.Integrations

open System
open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote
open CogniteSdk.Alpha
open Tests.Integration.Common
open Tests.Integration.Alpha.Common

let private now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
let private integrationExternalId = $"test_integration_{now}"
let private actionExternalId = $"test_action_{now}"
let private actionName = "test-action"

[<FactIf(envVar = "ENABLE_INTEGRATIONS_TESTS", skipReason = "Integrations API not enabled in this environment")>]
[<Trait("resource", "integrations")>]
let ``Create integration with actions, retrieve, list and cancel is Ok`` () =
    task {
        let integrationToCreate =
            CreateIntegration(ExternalId = integrationExternalId, Name = "test-integration")

        let! _ = writeClient.Alpha.Integrations.CreateAsync([ integrationToCreate ])

        try
            // Register available action via startup so the API accepts action creation
            let! _ =
                writeClient.Alpha.Integrations.StartupAsync(
                    StartupRequest(
                        ExternalId = integrationExternalId,
                        AvailableActions =
                            [ AvailableActionWrite(
                                  Name = actionName,
                                  Type = ActionType.custom,
                                  Description = "Test action"
                              ) ]
                    )
                )

            // Create an action
            let! created =
                writeClient.Alpha.Integrations.CreateActionsAsync(
                    integrationExternalId,
                    [ CreateAction(ExternalId = actionExternalId, ActionName = actionName) ]
                )

            let createdLen = Seq.length created
            test <@ createdLen = 1 @>
            let action = created |> Seq.head
            test <@ action.ExternalId = actionExternalId @>
            test <@ action.ActionName = actionName @>
            test <@ action.Status = ActionStatus.pending @>

            // Retrieve the action by external ID
            let! retrieved =
                writeClient.Alpha.Integrations.RetrieveActionsAsync([ actionExternalId ], false)

            test <@ Seq.length retrieved = 1 @>
            test <@ (retrieved |> Seq.head).ExternalId = actionExternalId @>

            // List actions for the integration
            let! listed =
                writeClient.Alpha.Integrations.ListActionsAsync(
                    ActionsQuery(Integration = integrationExternalId, IncludeCompleted = true)
                )

            test <@ Seq.length listed.Items >= 1 @>

            test
                <@
                    listed.Items
                    |> Seq.exists (fun a -> a.ExternalId = actionExternalId)
                @>

            // Cancel the action
            let! cancelled =
                writeClient.Alpha.Integrations.CancelActionsAsync([ actionExternalId ], false)

            test <@ Seq.length cancelled = 1 @>
            test <@ (cancelled |> Seq.head).Status = ActionStatus.cancel_pending @>

            // Check in reporting the cancellation as confirmed by the extractor
            let! checkIn =
                writeClient.Alpha.Integrations.CheckInAsync(
                    CheckInRequest(
                        ExternalId = integrationExternalId,
                        ActionUpdates =
                            [ ActionUpdate(
                                  ExternalId = actionExternalId,
                                  Status = ActionStatus.canceled
                              ) ]
                    )
                )

            test <@ checkIn.ExternalId = integrationExternalId @>
            ()
        finally
            writeClient.Alpha.Integrations.DeleteAsync([ integrationExternalId ], true)
                .GetAwaiter()
                .GetResult()
            |> ignore
    }
