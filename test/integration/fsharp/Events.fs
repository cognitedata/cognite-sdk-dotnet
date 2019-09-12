module Tests.Integration.Events

open System
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
    let externalIdString = Guid().ToString()
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

[<Fact>]
let ``Update assets is Ok`` () = async {
    // Arrange
    let wctx = writeCtx ()

    let externalIdString = "a new event external Id"
    let newMetadata = ([
        "key1", "value1"
        "key2", "value2"
    ]
    |> Map.ofList)
    let dto: EventWriteDto = {
        ExternalId = Some externalIdString
        StartTime = Some 1566815994L
        EndTime = Some 1566816009L
        Description = Some "dotnet sdk test"
        MetaData = [
            "oldkey1", "oldvalue1"
            "oldkey2", "oldvalue2"
        ] |> Map.ofList
        Source = None
        Type = None
        SubType = None
        AssetIds = []
    }
    let externalId = Identity.ExternalId externalIdString
    let newDescription = Some "UpdatedDesc"
    // Act
    let! createRes = Events.Create.createAsync [ dto ] wctx
    let! updateRes =
        Events.Update.updateAsync [
            (externalId, [
                EventUpdate.SetDescription newDescription
                EventUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
            ])
        ] wctx
    let! getRes = Events.Retrieve.getByIdsAsync [ externalId ] wctx
    let! delRes = Events.Delete.deleteAsync ([ externalId ]) wctx

    let resName, resExternalId, resMetaData =
        match getRes.Result with
        | Ok eventssResponses ->
            let h = Seq.tryHead eventssResponses
            match h with
            | Some eventResponse -> eventResponse.Description, eventResponse.ExternalId, eventResponse.MetaData
            | None -> Some "", Some "", Map.empty
        | Error _ -> Some "", Some "", Map.empty

    let updateSuccsess =
        match updateRes.Result with
        | Ok res -> true
        | Error _ -> false

    let metaDataOk =
        resMetaData.ContainsKey "key1"
        && resMetaData.ContainsKey "key2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    // Assert create
    test <@ Result.isOk createRes.Result @>
    test <@ createRes.Request.Method = HttpMethod.Post @>
    test <@ createRes.Request.Extra.["resource"] = "/events" @>
    test <@ createRes.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateSuccsess @>
    test <@ Result.isOk updateRes.Result @>
    test <@ updateRes.Request.Method = HttpMethod.Post @>
    test <@ updateRes.Request.Extra.["resource"] = "/events/update" @>
    test <@ updateRes.Request.Query.IsEmpty @>

    // Assert get
    test <@ Result.isOk getRes.Result @>
    test <@ getRes.Request.Method = HttpMethod.Post @>
    test <@ getRes.Request.Extra.["resource"] = "/events/byids" @>
    test <@ getRes.Request.Query.IsEmpty @>
    test <@ resExternalId = Some externalIdString @>
    test <@ resName = newDescription @>
    test <@ metaDataOk @>

    // Assert delete
    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = HttpMethod.Post @>
    test <@ delRes.Request.Extra.["resource"] = "/events/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}

[<Fact>]
let ``List events with limit is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let query = [ EventQuery.Limit 10 ]

    // Act
    let! res = Items.listAsync query [] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let filters = [
        EventFilter.AssetIds [ 4650652196144007L ]
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on subtype is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let filters = [
        EventFilter.Subtype "VAL"
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let subType =
        match res.Result with
        | Ok dtos ->
            Seq.head dtos.Items
            |> fun a -> a.SubType
        | Error _ -> None

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ subType = Some "VAL" @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Search events is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()

    let options = [
        EventSearch.Description "dotnet"
    ]

    // Act
    let retry = retry shouldRetry 100<ms> 15

    // Event is already created in test/integration/Test.CSharp.Integration/TestBase.cs
    let req = Events.Search.search 10 options [] |> retry
    let! res = req Async.single ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/search" @>
}