module Tests.Integration.Events

open System
open System.Net.Http
open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open Tests
open Common
open Oryx
open Oryx.Retry
open FSharp.Control.Tasks.V2.ContextInsensitive

open CogniteSdk
open CogniteSdk.Events

[<Fact>]
let ``Create and delete events is Ok`` () = task {
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
let ``Get event by id is Ok`` () = task {
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
let ``Get event by missing id is Error`` () = task {
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
let ``Get event by ids is Ok`` () = task {
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
let ``Update assets is Ok`` () = task {
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
let ``List events with limit is Ok`` () = task {
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
let ``Filter events is Ok`` () = task {
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
let ``Filter events on subtype is Ok`` () = task {
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
let ``Filter events on AssetIds is Ok`` () = task {
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

    let assetIds =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: EventReadDto) -> e.AssetIds) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall ((=) 4650652196144007L) assetIds @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on CreatedTime is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1554973225688L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1554973225708L)
    }
    let filters = [
        EventFilter.CreatedTime timerange
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let createdTimes =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: EventReadDto) -> e.CreatedTime) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1554973225708L && t > 1554973225688L) createdTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on LastUpdatedTime is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1554973225688L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1554973225708L)
    }
    let filters = [
        EventFilter.LastUpdatedTime timerange
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let lastUpdatedTimes =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: EventReadDto) -> e.CreatedTime) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1554973225708L && t > 1554973225688L) lastUpdatedTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on StartTime is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1565941319L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1565941339L)
    }
    let filters = [
        EventFilter.StartTime timerange
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let startTimes =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: EventReadDto) -> e.StartTime |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1565941339L && t > 1565941319L) startTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on EndTime is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1565941331L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1565941351L)
    }
    let filters = [
        EventFilter.EndTime timerange
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let endTimes =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: EventReadDto) -> e.EndTime |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1565941351L && t > 1565941331L) endTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let filters = [
        EventFilter.ExternalIdPrefix "odata"
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let externalIds =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: EventReadDto) -> e.ExternalId |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 2 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("odata")) externalIds @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on MetaData is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let filters = [
        EventFilter.MetaData (Map.ofList ["sourceId", "2758173488388242"])
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let ms =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: EventReadDto) -> e.MetaData) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun m -> Map.tryFind "sourceId" m = Some "2758173488388242") ms @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on Source is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let filters = [
        EventFilter.Source "akerbp-cdp"
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let sources =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: EventReadDto) -> e.Source |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "akerbp-cdp") sources @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Filter events on Type is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        EventQuery.Limit 10
    ]
    let filters = [
        EventFilter.Type "Monad"
    ]

    // Act
    let! res = Events.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let types =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: EventReadDto) -> e.Type |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "Monad") types @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/events/list" @>
}

[<Fact>]
let ``Search events is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()

    let options = [
        EventSearch.Description "dotnet"
    ]

    // Act
    let retry = retry 100<ms> 15

    // Event is already created in test/integration/Test.CSharp.Integration/TestBase.cs
    let req = Events.Search.search 10 options [] |> retry
    let! res = req Task.FromResult ctx

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