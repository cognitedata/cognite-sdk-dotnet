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

[<Fact>]
let ``Update sequences is Ok`` () = task {
    // Arrange
    let wctx = writeCtx ()

    let externalIdString = Guid.NewGuid().ToString();
    let newMetadata = ([
        "key1", "value1"
        "key2", "value2"
    ]
    |> Map.ofList)
    let dto: Sequences.SequenceCreateDto = {
        ExternalId = Some externalIdString
        Name = "Create Assets sdk test"
        Description = Some "dotnet sdk test"
        MetaData = [
            "oldkey1", "oldvalue1"
            "oldkey2", "oldvalue2"
        ] |> Map.ofList
        AssetId = 5409900891232494L

    }
    let externalId = Identity.ExternalId externalIdString
    let newName = "UpdatedName"
    let newExternalId = "updatedExternalId"

    // Act
    let! createRes = Assets.Create.createAsync [ dto ] wctx
    let! updateRes =
        Assets.Update.updateAsync [
            (externalId, [
                Assets.AssetUpdate.SetName newName
                Assets.AssetUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
                Assets.AssetUpdate.SetExternalId (Some newExternalId)
            ])
        ] wctx
    let! getRes = Assets.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx' =
        match getRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resName, resExternalId, resMetaData =
        let assetsResponses = getCtx'.Response
        let h = Seq.tryHead assetsResponses
        match h with
        | Some assetResponse -> assetResponse.Name, assetResponse.ExternalId, assetResponse.MetaData
        | None -> "", Some "", Map.empty

    let updateSuccsess = Result.isOk updateRes

    let metaDataOk =
        (Map.tryFind "key1" resMetaData) = Some "value1"
        && (Map.tryFind "key2" resMetaData) = Some "value2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    let createCtx' =
        match createRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let updateCtx' =
        match updateRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    // Assert create
    test <@ createCtx'.Request.Method = HttpMethod.Post @>
    test <@ createCtx'.Request.Extra.["resource"] = "/assets" @>
    test <@ createCtx'.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateSuccsess @>
    test <@ updateCtx'.Request.Method = HttpMethod.Post @>
    test <@ updateCtx'.Request.Extra.["resource"] = "/assets/update" @>
    test <@ updateCtx'.Request.Query.IsEmpty @>

    // Assert get
    test <@ getCtx'.Request.Method = HttpMethod.Post @>
    test <@ getCtx'.Request.Extra.["resource"] = "/assets/byids" @>
    test <@ getCtx'.Request.Query.IsEmpty @>
    test <@ resExternalId = Some "updatedExternalId" @>
    test <@ resName = newName @>
    test <@ metaDataOk @>

    let newDescription = "updatedDescription"
    let newSource = "updatedSource"

    let! updateRes2 =
        Assets.Update.updateAsync [
            (Identity.ExternalId newExternalId, [
                Assets.AssetUpdate.SetMetaData (Map.ofList ["newKey", "newValue"])
                Assets.AssetUpdate.SetDescription (Some newDescription)
                Assets.AssetUpdate.SetSource newSource
            ])
        ] wctx

    let! getRes2 = Assets.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx2' =
        match getRes2 with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resDescription, resSource, resMetaData2, identity =
        let assetsResponses = getCtx2'.Response
        let h = Seq.tryHead assetsResponses
        match h with
        | Some assetResponse ->
            assetResponse.Description, assetResponse.Source, assetResponse.MetaData, assetResponse.Id
        | None -> Some "", Some "", Map.empty, 0L

    // Assert get2
    test <@ getCtx2'.Request.Method = HttpMethod.Post @>
    test <@ getCtx2'.Request.Extra.["resource"] = "/assets/byids" @>
    test <@ getCtx2'.Request.Query.IsEmpty @>
    test <@ resDescription = Some newDescription @>
    test <@ resSource = Some newSource @>
    test <@ (Map.tryFind "newKey" resMetaData2) = Some "newValue" @>

    let! updateRes3 =
        Assets.Update.updateAsync [
            (Identity.Id identity, [
                Assets.AssetUpdate.ChangeMetaData (Map.empty, ["newKey"])
                Assets.AssetUpdate.ClearExternalId
                Assets.AssetUpdate.ClearSource
            ])
        ] wctx

    let! getRes3 = Assets.Retrieve.getByIdsAsync [ Identity.Id identity ] wctx
    let! delRes = Assets.Delete.deleteAsync ([ Identity.Id identity], false) wctx

    let getCtx3' =
        match getRes3 with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resExternalId2, resSource2, resMetaData3 =
        let assetsResponses = getCtx3'.Response
        let h = Seq.tryHead assetsResponses
        match h with
        | Some assetResponse ->
            assetResponse.ExternalId, assetResponse.Source, assetResponse.MetaData
        | None -> Some "", Some "", Map.empty

    let delCtx =
        match delRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    // Assert get2
    test <@ getCtx2'.Request.Method = HttpMethod.Post @>
    test <@ getCtx2'.Request.Extra.["resource"] = "/assets/byids" @>
    test <@ getCtx2'.Request.Query.IsEmpty @>
    test <@ resExternalId2 = None @>
    test <@ resSource2 = None @>
    test <@ Map.isEmpty resMetaData3 @>

    // Assert delete
    test <@ delCtx.Request.Method = HttpMethod.Post @>
    test <@ delCtx.Request.Extra.["resource"] = "/assets/delete" @>
    test <@ delCtx.Request.Query.IsEmpty @>
}
