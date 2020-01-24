module Tests.Integration.Sequences

open System
open System.Net.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Xunit
open Swensen.Unquote

open Oryx

open CogniteSdk
open CogniteSdk.Sequences

open Common

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
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

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
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

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
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    test <@ len = 1 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/byids" @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Get sequence rows by ids is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let sequencesId = 5702374195409554L |> Identity.Id
    let expectedRows =
        [
            { RowNumber = 1L; Values = [ RowValue.String "row1"] }
            { RowNumber = 2L; Values = [ RowValue.String "row2"] }
        ] |> Seq.ofList
    // Act
    let! res = Sequences.Rows.Items.listRowsAsync sequencesId [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dto = ctx'.Response
    let colLength = Seq.length dto.Columns
    let rowLength = Seq.length dto.Rows

    test <@ rowLength = 2 @>
    test <@ colLength = 1 @>
    test <@ dto.Id = 5702374195409554L @>
    test <@ dto.Rows = expectedRows @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/data/list" @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Create and delete sequences is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let columnExternalIdString = Guid.NewGuid().ToString()
    let externalIdString = Guid.NewGuid().ToString()
    let name = Guid.NewGuid().ToString()
    let column: ColumnWriteDto = {
        Name = Some "Create column sdk test"
        ExternalId = columnExternalIdString
        Description = Some "dotnet sdk test"
        ValueType = ValueType.Double
        Metadata = Map.empty
    }
    let dto: Sequences.SequenceWriteDto = {
        ExternalId = Some externalIdString
        Name = Some name
        Description = Some "dotnet sdk test"
        Metadata = Map.empty
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
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let delCtx' =
        match delRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resExternalId =
        match res with
        | Ok ctx' ->
            let sequencesResponses = ctx'.Response
            let h = Seq.tryHead sequencesResponses
            match h with
            | Some sequenceResponse -> sequenceResponse.ExternalId
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
let ``Create and delete sequences rows is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let columnExternalIdString = Guid.NewGuid().ToString()
    let externalIdString = Guid.NewGuid().ToString()
    let externalId = Identity.ExternalId externalIdString
    let name = Guid.NewGuid().ToString()
    let column: ColumnWriteDto = {
        Name = Some "Create column sdk test"
        ExternalId = columnExternalIdString
        Description = Some "dotnet sdk test"
        ValueType = ValueType.String
        MetaData = Map.empty
    }
    let dto: Sequences.SequenceWriteDto = {
        ExternalId = Some externalIdString
        Name = Some name
        Description = Some "dotnet sdk test"
        MetaData = Map.empty
        AssetId = None
        Columns = [column]
    }
    let deleteDto: Sequences.Rows.Delete.SequenceDataDelete = {
        Rows = []
        Id = externalId
    }

    let rows: RowDto seq =
        [
            { RowNumber = 1L; Values = [RowValue.String "row1"] }
            { RowNumber = 2L; Values = [RowValue.String "row2"] }
        ] |> Seq.ofList
    let rowDto: Rows.Insert.SequenceDataWriteDto = {
        Columns = ["sdk-column"]
        Rows = rows
        Id = Identity.Id 5702374195409554L
    }
    let externalId = Identity.ExternalId externalIdString

    // Act
    let! res = Sequences.Create.createAsync [ dto ] ctx
    let! rowRes = Sequences.Rows.Insert.insertRowsAsync [ rowDto ] ctx
    let! rowDelRes = Sequences.Rows.Delete.deleteRowsAsync [ deleteDto ] ctx
    let! delRes = Sequences.Delete.deleteAsync [ externalId ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let delCtx' =
        match delRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let rowCtx' =
        match rowRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let rowDelCtx' =
        match rowDelRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resExternalId =
        match res with
        | Ok ctx' ->
            let sequencesResponses = ctx'.Response
            let h = Seq.tryHead sequencesResponses
            match h with
            | Some sequenceResponse -> sequenceResponse.ExternalId
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

    test <@ resExternalId = Some externalIdString @>
    test <@ rowCtx'.Request.Method = HttpMethod.Post @>
    test <@ rowCtx'.Request.Extra.["resource"] = "/sequences/data" @>
    test <@ rowCtx'.Request.Query.IsEmpty @>

    test <@ rowDelCtx'.Request.Method = HttpMethod.Post @>
    test <@ rowDelCtx'.Request.Extra.["resource"] = "/sequences/data/delete" @>
    test <@ rowDelCtx'.Request.Query.IsEmpty @>
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
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/search" @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Update sequences is Ok`` () = task {
    // Arrange
    let wctx = writeCtx ()

    let columnExternalIdString = Guid.NewGuid().ToString();
    let externalIdString = Guid.NewGuid().ToString();
    let newMetadata = ([
        "key1", "value1"
        "key2", "value2"
    ]
    |> Map.ofList)
    let column: ColumnWriteDto = {
        Name = Some "Create column sdk test"
        ExternalId = columnExternalIdString
        Description = Some "dotnet sdk test"
        ValueType = ValueType.Double
        Metadata = Map.empty
    }
    let dto: Sequences.SequenceWriteDto = {
        ExternalId = Some externalIdString
        Name = Some "Create Sequences sdk test"
        Description = Some "dotnet sdk test"
        Metadata = [
            "oldkey1", "oldvalue1"
            "oldkey2", "oldvalue2"
        ] |> Map.ofList
        AssetId = Some 5409900891232494L
        Columns = [column]
    }
    let externalId = Identity.ExternalId externalIdString
    let newName = "UpdatedName"
    let newExternalId = Guid.NewGuid().ToString();

    // Act
    let! createRes = Sequences.Create.createAsync [ dto ] wctx
    let! updateRes =
        Sequences.Update.updateAsync [
            (externalId, [
                Sequences.SequenceUpdate.SetName (Some newName)
                Sequences.SequenceUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
                Sequences.SequenceUpdate.SetExternalId (Some newExternalId)
            ])
        ] wctx
    let! getRes = Sequences.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx' =
        match getRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resName, resExternalId, resMetaData =
        let sequencesResponses = getCtx'.Response
        let h = Seq.tryHead sequencesResponses
        match h with
        | Some sequenceResponse -> sequenceResponse.Name, sequenceResponse.ExternalId, sequenceResponse.MetaData
        | None -> Some "", Some "", Map.empty

    let updateSuccsess = Result.isOk updateRes

    let metaDataOk =
        (Map.tryFind "key1" resMetaData) = Some "value1"
        && (Map.tryFind "key2" resMetaData) = Some "value2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    let createCtx' =
        match createRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let updateCtx' =
        match updateRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert create
    test <@ createCtx'.Request.Method = HttpMethod.Post @>
    test <@ createCtx'.Request.Extra.["resource"] = "/sequences" @>
    test <@ createCtx'.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateSuccsess @>
    test <@ updateCtx'.Request.Method = HttpMethod.Post @>
    test <@ updateCtx'.Request.Extra.["resource"] = "/sequences/update" @>
    test <@ updateCtx'.Request.Query.IsEmpty @>

    // Assert get
    test <@ getCtx'.Request.Method = HttpMethod.Post @>
    test <@ getCtx'.Request.Extra.["resource"] = "/sequences/byids" @>
    test <@ getCtx'.Request.Query.IsEmpty @>
    test <@ resExternalId = Some newExternalId @>
    test <@ resName = Some newName @>
    test <@ metaDataOk @>

    let newDescription = "updatedDescription"

    let! updateRes2 =
        Sequences.Update.updateAsync [
            (Identity.ExternalId newExternalId, [
                Sequences.SequenceUpdate.SetMetaData (Map.ofList ["newKey", "newValue"])
                Sequences.SequenceUpdate.SetDescription (Some newDescription)
                Sequences.SequenceUpdate.SetAssetId (Some 5409900891232494L)
            ])
        ] wctx

    let! getRes2 = Sequences.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx2' =
        match getRes2 with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resDescription, resAssetId, resMetaData2, identity =
        let sequencesResponses = getCtx2'.Response
        let h = Seq.tryHead sequencesResponses
        match h with
        | Some sequenceResponse ->
            sequenceResponse.Description, sequenceResponse.AssetId, sequenceResponse.MetaData, sequenceResponse.Id
        | None -> Some "", Some 0L, Map.empty, 0L

    // Assert get2
    test <@ getCtx2'.Request.Method = HttpMethod.Post @>
    test <@ getCtx2'.Request.Extra.["resource"] = "/sequences/byids" @>
    test <@ getCtx2'.Request.Query.IsEmpty @>
    test <@ resDescription = Some newDescription @>
    test <@ resAssetId = Some 5409900891232494L @>
    test <@ (Map.tryFind "newKey" resMetaData2) = Some "newValue" @>

    let! updateRes3 =
        Sequences.Update.updateAsync [
            (Identity.Id identity, [
                Sequences.SequenceUpdate.ChangeMetaData (Map.empty, ["newKey"])
                Sequences.SequenceUpdate.ClearExternalId
                Sequences.SequenceUpdate.SetAssetId None
            ])
        ] wctx

    let! getRes3 = Sequences.Retrieve.getByIdsAsync [ Identity.Id identity ] wctx
    let! delRes = Sequences.Delete.deleteAsync [ Identity.Id identity] wctx

    let getCtx3' =
        match getRes3 with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resExternalId2, resAssetId2, resMetaData3 =
        let sequencesResponses = getCtx3'.Response
        let h = Seq.tryHead sequencesResponses
        match h with
        | Some sequenceResponse ->
            sequenceResponse.ExternalId, sequenceResponse.AssetId, sequenceResponse.MetaData
        | None -> Some "", Some 0L, Map.empty

    let delCtx =
        match delRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert get2
    test <@ getCtx2'.Request.Method = HttpMethod.Post @>
    test <@ getCtx2'.Request.Extra.["resource"] = "/sequences/byids" @>
    test <@ getCtx2'.Request.Query.IsEmpty @>
    test <@ resExternalId2 = None @>
    test <@ resAssetId2 = None @>
    test <@ Map.isEmpty resMetaData3 @>

    // Assert delete
    test <@ delCtx.Request.Method = HttpMethod.Post @>
    test <@ delCtx.Request.Extra.["resource"] = "/sequences/delete" @>
    test <@ delCtx.Request.Query.IsEmpty @>
}
