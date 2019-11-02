module Tests.Integration.Sequences

open System
open System.Net.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Xunit
open Swensen.Unquote

open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.Sequences
open Common
open System.Threading.Tasks

[<Trait("resource", "sequences")>]
[<Fact>]
let ``List Sequences with limit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ SequenceQuery.Limit 10 ]

    // Act
    let! res = Items.listAsync query [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/list" @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``List Sequences with limit and filter is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ SequenceQuery.Limit 10 ]
    let filter = [ SequenceFilter.Name "sdk-test-sequence" ]

    // Act
    let! res = Items.listAsync query filter ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/list" @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Get sequences by ids is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let sequencesIds =
        [ 5702374195409554L ]
        |> Seq.map Identity.Id

    // Act
    let! res = Sequences.Retrieve.getByIdsAsync sequencesIds ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    test <@ len = 1 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/byids" @>
}

let ``Create and delete assets is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let columnExternalIdString = Guid.NewGuid().ToString()
    let externalIdString = Guid.NewGuid().ToString()
    let name = Guid.NewGuid().ToString()
    let column: ColumnCreateDto = {
        Name = Some "Create column sdk test"
        ExternalId = columnExternalIdString
        Description = Some "dotnet sdk test"
        ValueType = ValueType.Double
        MetaData = Map.empty
    }
    let dto: Sequences.SequenceCreateDto = {
        ExternalId = Some externalIdString
        Name = Some name
        Description = Some "dotnet sdk test"
        MetaData = Map.empty
        AssetId = None
        Columns = [column]
    }
    let externalId = Identity.ExternalId externalIdString

    // Act
    let! res = Sequences.Create.createAsync [ dto ] ctx
    let! delRes = Sequences.Delete.deleteAsync [ externalId ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let delCtx' =
        match delRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resExternalId =
        match res with
        | Ok ctx' ->
            let assetsResponses = ctx'.Response
            let h = Seq.tryHead assetsResponses
            match h with
            | Some assetsResponse -> assetsResponse.ExternalId
            | None -> None
        | Error _ -> None

    // Assert
    test <@ resExternalId = Some externalIdString @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences" @>
    test <@ ctx'.Request.Query.IsEmpty @>

    test <@ delCtx'.Request.Method = HttpMethod.Post @>
    test <@ delCtx'.Request.Extra.["resource"] = "/sequences/delete" @>
    test <@ delCtx'.Request.Query.IsEmpty @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Search sequences is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        Sequences.SequenceSearch.Name "sdk-test"
    ]

    // Act
    let! res = Search.searchAsync 10 options [] ctx
    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/search" @>
}
