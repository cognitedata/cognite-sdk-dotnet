module Tests.Integration.Events

open Xunit
open Swensen.Unquote

open Fusion
open Tests
open Common
open CogniteSdk
open CogniteSdk.Events
open System.Net.Http

[<Fact>]
let ``Create and delete events is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = "createDeleteTestEvents"
    let dto: Events.WriteDto = {
        ExternalId = Some externalIdString
        StartTime = Some 1565941329L
        EndTime = Some 1565941341L
        Type = Some "dotnet test"
        SubType = Some "create and delete"
        Description = Some "dotnet sdk test"
        MetaData = Map.empty
        AssetIds = Seq.empty
        Source = None
    }
    let externalId = Identity.ExternalId externalIdString

    // Act
    let! res = Events.Create.createAsync [ dto ] ctx
    let! delRes = Events.Delete.deleteAsync ([ externalId ]) ctx
    let resExternalId =
        match res.Result with
        | Ok eventsResponses ->
            let h = Seq.tryHead eventsResponses
            match h with
            | Some eventsResponse -> eventsResponse.ExternalId
            | None -> None
        | Error _ -> None

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resExternalId = Some externalIdString @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events" @>
    test <@ res.Request.Query.IsEmpty @>

    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = HttpMethod.Post @>
    test <@ delRes.Request.Extra.["resource"] = "/events/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}
