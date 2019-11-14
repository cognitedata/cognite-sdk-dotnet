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
    let externalIdString = Guid.NewGuid().ToString()
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let delCtx' =
        match delRes with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resExternalId =
        let eventsResponses = ctx'.Response
        let h = Seq.tryHead eventsResponses
        match h with
        | Some eventsResponse -> eventsResponse.ExternalId
        | None -> None

    // Assert
    test <@ resExternalId = Some externalIdString @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events" @>
    test <@ ctx'.Request.Query.IsEmpty @>

    test <@ delCtx'.Request.Method = HttpMethod.Post @>
    test <@ delCtx'.Request.Extra.["resource"] = "/events/delete" @>
    test <@ delCtx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get event by id is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let eventId = 19442413705355L

    // Act
    let! res = Events.Entity.getAsync eventId ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dto = ctx'.Response
    let resId = dto.Id

    // Assert
    test <@ resId = eventId @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/19442413705355" @>
}

[<Fact>]
let ``Get event by missing id is Error`` () = task {
    // Arrange
    let ctx = readCtx ()
    let eventId = 0L

    // Act
    let! res = Events.Entity.getAsync eventId ctx

    let err =
        match res with
        | Ok _ -> ResponseError.empty
        | Error (ApiError err) -> err
        | Error (Panic err) -> raise err

    // Assert
    test <@ Result.isError res @>
    test <@ err.Code = 400 @>
    test <@ err.Message.Contains "constraint violations" @>
    test <@ Option.isSome err.RequestId @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len = 3 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/byids" @>
}

[<Fact>]
let ``Update assets is Ok`` () = task {
    // Arrange
    let wctx = writeCtx ()

    let externalIdString = Guid.NewGuid().ToString();
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

    let getCtx' =
        match getRes with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resName, resExternalId, resMetaData =
        let eventssResponses = getCtx'.Response
        let h = Seq.tryHead eventssResponses
        match h with
        | Some eventResponse -> eventResponse.Description, eventResponse.ExternalId, eventResponse.MetaData
        | None -> Some "", Some "", Map.empty

    let updateCtx' =
        match updateRes with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let updateSuccsess = Result.isOk updateRes

    let metaDataOk =
        resMetaData.ContainsKey "key1"
        && resMetaData.ContainsKey "key2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    let createCtx' =
        match createRes with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let delCtx' =
        match delRes with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert create
    test <@ createCtx'.Request.Method = HttpMethod.Post @>
    test <@ createCtx'.Request.Extra.["resource"] = "/events" @>
    test <@ createCtx'.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateSuccsess @>
    test <@ updateCtx'.Request.Method = HttpMethod.Post @>
    test <@ updateCtx'.Request.Extra.["resource"] = "/events/update" @>
    test <@ updateCtx'.Request.Query.IsEmpty @>

    // Assert get
    test <@ getCtx'.Request.Method = HttpMethod.Post @>
    test <@ getCtx'.Request.Extra.["resource"] = "/events/byids" @>
    test <@ getCtx'.Request.Query.IsEmpty @>
    test <@ resExternalId = Some externalIdString @>
    test <@ resName = newDescription @>
    test <@ metaDataOk @>

    // Assert delete
    test <@ delCtx'.Request.Method = HttpMethod.Post @>
    test <@ delCtx'.Request.Extra.["resource"] = "/events/delete" @>
    test <@ delCtx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``List events with limit is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let query = [ EventQuery.Limit 10 ]

    // Act
    let! res = Items.listAsync query [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let subType = Seq.head dtos.Items |> fun a -> a.SubType

    // Assert
    test <@ len = 10 @>
    test <@ subType = Some "VAL" @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let assetIds = Seq.collect (fun (e: EventReadDto) -> e.AssetIds) dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) 4650652196144007L) assetIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let createdTimes = Seq.map (fun (e: EventReadDto) -> e.CreatedTime) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1554973225708L && t > 1554973225688L) createdTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let lastUpdatedTimes = Seq.map (fun (e: EventReadDto) -> e.CreatedTime) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1554973225708L && t > 1554973225688L) lastUpdatedTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let startTimes = Seq.collect (fun (e: EventReadDto) -> e.StartTime |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1565941339L && t > 1565941319L) startTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let endTimes = Seq.collect (fun (e: EventReadDto) -> e.EndTime |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1565941351L && t > 1565941331L) endTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let externalIds = Seq.collect (fun (e: EventReadDto) -> e.ExternalId |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 2 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("odata")) externalIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let ms = Seq.map (fun (e: EventReadDto) -> e.MetaData) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun m -> Map.tryFind "sourceId" m = Some "2758173488388242") ms @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let sources = Seq.collect (fun (e: EventReadDto) -> e.Source |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "akerbp-cdp") sources @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let types = Seq.collect (fun (e: EventReadDto) -> e.Type |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "Monad") types @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/list" @>
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
    let! res = req finishEarly ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len = 1 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/events/search" @>
}