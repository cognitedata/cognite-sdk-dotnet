module Tests.Integration.Events

open System
open System.Collections.Generic

open FSharp.Control.Tasks.V2.ContextInsensitive
open Xunit
open Swensen.Unquote

open Common

open CogniteSdk
open CogniteSdk.Events

[<Fact>]
let ``Create and delete events is Ok`` () = task {
    // Arrange
    let externalId = Guid.NewGuid().ToString()
    let dto =
        EventWriteDto(
            ExternalId = externalId,
            StartTime = Nullable 1565941329L,
            EndTime = Nullable 1565941341L,
            Type = "dotnet test",
            Subtype = "create and delete",
            Description = "dotnet sdk test"
        )

    // Act
    let! res = writeClient.Events.CreateAsync [ dto ]
    let! delRes = writeClient.Events.DeleteAsync [ externalId ]

    // Assert
    test <@ res |> Seq.forall (fun ev -> ev.ExternalId = externalId) @>
}

[<Fact>]
let ``Get event by id is Ok`` () = task {
    // Arrange
    let eventId = 19442413705355L

    // Act
    let! res = readClient.Events.GetAsync eventId

    let resId = res.Id

    // Assert
    test <@ resId = eventId @>
}

[<Fact>]
let ``Get event by missing id is Error`` () = task {
    // Arrange
    let eventId = 0L

    // Act
    let! res =
        task {
            try
                let! a = readClient.Events.GetAsync eventId
                return Ok a
            with
            | :? ResponseException as e -> return Error e
        }

    let err = Result.getError res

    // Assert
    test <@ Result.isError res @>
    test <@ err.Code = 400 @>
    test <@ err.Message.Contains "constraint violations" @>
    test <@ not (isNull err.RequestId) @>
}

[<Fact>]
let ``Get event by ids is Ok`` () = task {
    // Arrange
    let eventIds =
        [ 1995162693488L; 6959277162251L; 13821390033633L ]

    // Act
    let! res = readClient.Events.RetrieveAsync eventIds

    let len = Seq.length res

    // Assert
    test <@ len = 3 @>
}

[<Fact>]
let ``Update events is Ok`` () = task {
    // Arrange
    let externalId = Guid.NewGuid().ToString();
    let newMetadata = Dictionary(dict [
        "key1", "value1"
        "key2", "value2"
    ])
    let dto =
        EventWriteDto(
            ExternalId = externalId,
            StartTime = Nullable 1566815994L,
            EndTime = Nullable 1566816009L,
            Description = "dotnet sdk test",
            Metadata = Dictionary(dict [ "oldkey1", "oldvalue1"; "oldkey2", "oldvalue2" ])
        )
    let newDescription = "UpdatedDesc"
    // Act
    let! createRes = writeClient.Events.CreateAsync [ dto ]
    let! updateRes =
        writeClient.Events.UpdateAsync [
            EventUpdateItem(
                externalId = externalId,
                Update = EventUpdateDto(
                    Description = Update(newDescription),
                    Metadata = DictUpdate<string>(add=newMetadata, remove=[ "oldkey1" ])
                )
            )
        ]

    let! getRes = writeClient.Events.RetrieveAsync [ externalId ]
    let! delRes = writeClient.Events.DeleteAsync [ externalId ]

    let getDto = Seq.head getRes

    let metaDataOk =
        getDto.Metadata.ContainsKey "key1"
        && getDto.Metadata.ContainsKey "key2"
        && getDto.Metadata.ContainsKey "oldkey2"
        && not (getDto.Metadata.ContainsKey "oldkey1")

    // Assert get
    test <@ getDto.ExternalId = externalId @>
    test <@ getDto.Description = newDescription @>
    test <@ getDto.Metadata |> fun a -> true && metaDataOk @>
}

[<Fact>]
let ``List events with limit is Ok`` () = task {
    // Arrange
    let query = EventQueryDto(Limit = Nullable 10)

    // Act
    let! res = readClient.Events.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 10 @>
}

[<Fact>]
let ``Filter events on AssetIds is Ok`` () = task {
    // Arrange
    let filter = EventFilterDto(AssetIds = [ 4650652196144007L ])
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Events.ListAsync query

    let len = Seq.length res.Items
    let assetIds = Seq.collect (fun (e: EventReadDto) -> e.AssetIds) res.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) 4650652196144007L) assetIds @>
}

[<Fact>]
let ``Filter events on subtype is Ok`` () = task {
    // Arrange
    let filter = EventFilterDto(Subtype = "VAL")
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Events.ListAsync query

    let len = Seq.length res.Items

    let subTypes = res.Items |> Seq.map (fun a -> a.Subtype.ToUpper())

    // Assert
    test <@ len = 10 @>
    test <@ subTypes |> Seq.forall ((=) "VAL") @>
}

[<Fact>]
let ``Filter events on CreatedTime is Ok`` () = task {
    // Arrange
    let timerange = TimeRange(Min = Nullable 1554973225688L, Max = Nullable 1554973225708L)
    let filter = EventFilterDto(CreatedTime = timerange)
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Events.ListAsync query

    let len = Seq.length res.Items

    let createdTimes = Seq.map (fun (e: EventReadDto) -> e.CreatedTime) res.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1554973225708L && t > 1554973225688L) createdTimes @>
}

[<Fact>]
let ``Filter events on LastUpdatedTime is Ok`` () = task {
    // Arrange
    let timerange = TimeRange(Min = Nullable 1554973225688L, Max = Nullable 1554973225708L)
    let filter = EventFilterDto(LastUpdatedTime = timerange)
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Events.ListAsync query
    let len = Seq.length res.Items

    let lastUpdatedTimes = Seq.map (fun (e: EventReadDto) -> e.CreatedTime) res.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1554973225708L && t > 1554973225688L) lastUpdatedTimes @>
}

[<Fact>]
let ``Filter events on StartTime is Ok`` () = task {
    // Arrange
    let timerange = TimeRange(Min = Nullable 1565941319L, Max = Nullable 1565941339L)
    let filter = EventFilterDto(StartTime = timerange)
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Events.ListAsync query

    let len = Seq.length res.Items

    let startTimes = Seq.map (fun (e: EventReadDto) -> e.StartTime) res.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (t: Nullable<int64>) -> t.Value < 1565941339L && t.Value > 1565941319L) startTimes @>
}

[<Fact>]
let ``Filter events on EndTime is Ok`` () = task {
    // Arrange
    let timerange = TimeRange(Min = Nullable 1565941331L, Max = Nullable 1565941351L)
    let filter = EventFilterDto(EndTime = timerange)
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Events.ListAsync query
    let len = Seq.length res.Items

    let endTimes = Seq.map (fun (e: EventReadDto) -> e.EndTime) res.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (t: Nullable<int64>) -> t.Value < 1565941351L && t.Value > 1565941331L) endTimes @>
}

[<Fact>]
let ``Filter events on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let filter = EventFilterDto(ExternalIdPrefix = "odata")
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Events.ListAsync query

    let externalIds = Seq.map (fun (e: EventReadDto) -> e.ExternalId) res.Items

    // Assert
    test <@ Seq.length externalIds = 3 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("odata")) externalIds @>
}

[<Fact>]
let ``Filter events on Metadata is Ok`` () = task {
    // Arrange
    let filter = EventFilterDto(Metadata = Dictionary(dict ["sourceId", "2758173488388242"]))
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Events.ListAsync query
    let len = Seq.length res.Items

    let ms = Seq.map (fun (e: EventReadDto) -> e.Metadata) res.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (m: Dictionary<string, string>) -> m.Item "sourceId" = "2758173488388242") ms @>
}

[<Fact>]
let ``Filter events on Source is Ok`` () = task {
    // Arrange
    let filter = EventFilterDto(Source = "akerbp-cdp")
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Events.ListAsync query
    let len = Seq.length res.Items

    let sources = Seq.map (fun (e: EventReadDto) -> e.Source) res.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "akerbp-cdp") sources @>
}

[<Fact>]
let ``Filter events on Type is Ok`` () = task {
    // Arrange
    let filter = EventFilterDto(Type = "Monad")
    let query = EventQueryDto(Limit = Nullable 10, Filter = filter)
    // Act
    let! res = writeClient.Events.ListAsync query
    let len = Seq.length res.Items

    let types = Seq.map (fun (e: EventReadDto) -> e.Type) res.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "Monad") types @>
}

[<Fact>]
let ``Search events is Ok`` () = task {
    // Arrange
    let dto = DescriptionSearchDto(Description = "dotnet")
    let query = EventSearchDto(Search = dto)

    // Act
    let! res = writeClient.Events.SearchAsync query
    let len = Seq.length res

    // Assert
    test <@ len = 1 @>
}
