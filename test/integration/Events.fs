module Tests.Integration.Events

open Xunit
open Swensen.Unquote

open Tests
open Common
open CogniteSdk
open CogniteSdk.Events
open System.Net.Http
open Oryx

[<Fact>]
let ``Create and delete events is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = "createDeleteTestEvents"
    let dto: Events.EventWriteDto = {
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

[<Fact>]
let ``Get event by id is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let eventId = 19442413705355L

    // Act
    let! res = Events.Entity.getAsync eventId ctx

    let resId =
        match res.Result with
        | Ok dto -> dto.Id
        | Error _ -> 0L

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = eventId @>
    test <@ res.Request.Method = HttpMethod.Get @>
    test <@ res.Request.Extra.["resource"] = "/events/19442413705355" @>
}

[<Fact>]
let ``Get event by missing id is Error`` () = async {
    // Arrange
    let ctx = readCtx ()
    let eventId = 0L

    // Act
    let! res = Events.Entity.getAsync eventId ctx

    let err =
        match res.Result with
        | Ok _ -> ResponseError.empty
        | Error err -> err

    // Assert
    test <@ Result.isError res.Result @>
    test <@ err.Code = 400 @>
    test <@ err.Message = "getByInternalId.arg0: must be greater than or equal to 1" @>
}

[<Fact>]
let ``Get event by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let eventIds =
        [ 1995162693488L; 6959277162251L; 13821390033633L ]
        |> Seq.map Identity.Id

    // Act
    let! res = Events.Retrieve.getByIdsAsync eventIds ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 3 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/byids" @>
}