module Tests.Integration.Assets

open System.Net.Http

open Swensen.Unquote
open Xunit


open Oryx
open CogniteSdk
open CogniteSdk.Assets
open Common
open Tests

[<Fact>]
let ``List assets with limit is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let query = [ AssetQuery.Limit 10 ]

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
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Get asset by id is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let assetId = 130452390632424L

    // Act
    let! res = Assets.Entity.getAsync assetId ctx

    let resId =
        match res.Result with
        | Ok dto -> dto.Id
        | Error _ -> 0L

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = assetId @>
    test <@ res.Request.Method = HttpMethod.Get @>
    test <@ res.Request.Extra.["resource"] = "/assets/130452390632424" @>
}

[<Fact>]
let ``Get asset by missing id is Error`` () = async {
    // Arrange
    let ctx = readCtx ()
    let assetId = 0L

    // Act
    let! res = Assets.Entity.getAsync assetId ctx

    let err =
        match res.Result with
        | Ok _ -> ResponseError.empty
        | Error err -> err

    // Assert
    test <@ Result.isError res.Result @>
    test <@ err.Code = 400 @>
    test <@ err.Message = "getAsset.arg0: must be greater than or equal to 1" @>
}

[<Fact>]
let ``Get asset by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let assetIds =
        [ 130452390632424L; 126847700303897L; 124419735577853L ]
        |> Seq.map Identity.Id

    // Act
    let! res = Assets.Retrieve.getByIdsAsync assetIds ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 3 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/byids" @>
}

[<Fact>]
let ``Search assets is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AssetSearch.Name "23"
    ]

    // Act
    let! res = Assets.Search.searchAsync 10 options [] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Filter assets is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.RootIds [ Identity.Id 6687602007296940L ]
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Create and delete assets is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = "createDeleteTestAssets"
    let dto: AssetWriteDto = {
        ExternalId = Some externalIdString
        Name = "Create Assets sdk test"
        ParentId = None
        Description = Some "dotnet sdk test"
        MetaData = Map.empty
        Source = None
        ParentExternalId = None
    }
    let externalId = Identity.ExternalId externalIdString

    // Act
    let! res = Assets.Create.createAsync [ dto ] ctx
    let! delRes = Assets.Delete.deleteAsync ([ externalId ], false) ctx
    let resExternalId =
        match res.Result with
        | Ok assetsResponses ->
            let h = Seq.tryHead assetsResponses
            match h with
            | Some assetsResponse -> assetsResponse.ExternalId
            | None -> None
        | Error _ -> None

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resExternalId = Some externalIdString @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets" @>
    test <@ res.Request.Query.IsEmpty @>

    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = HttpMethod.Post @>
    test <@ delRes.Request.Extra.["resource"] = "/assets/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Update assets is Ok`` () = async {
    // Arrange
    let wctx = writeCtx ()

    let externalIdString = "createDeleteTestAssets"
    let newMetadata = ([
        "key1", "value1"
        "key2", "value2"
    ]
    |> Map.ofList)
    let dto: AssetWriteDto = {
        ExternalId = Some externalIdString
        Name = "Create Assets sdk test"
        ParentId = None
        Description = Some "dotnet sdk test"
        MetaData = [
            "oldkey1", "oldvalue1"
            "oldkey2", "oldvalue2"
        ] |> Map.ofList
        Source = None
        ParentExternalId = None
    }
    let externalId = Identity.ExternalId externalIdString
    let newName = "UpdatedName"
    // Act
    let! createRes = Assets.Create.createAsync [ dto ] wctx
    let! updateRes =
        Assets.Update.updateAsync [
            (externalId, [
                AssetUpdate.SetName newName
                AssetUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
            ])
        ] wctx
    let! getRes = Assets.Retrieve.getByIdsAsync [ externalId ] wctx
    let! delRes = Assets.Delete.deleteAsync ([ externalId ], false) wctx

    let resName, resExternalId, resMetaData =
        match getRes.Result with
        | Ok assetsResponses ->
            let h = Seq.tryHead assetsResponses
            match h with
            | Some assetResponse -> assetResponse.Name, assetResponse.ExternalId, assetResponse.MetaData
            | None -> "", Some "", Map.empty
        | Error _ -> "", Some "", Map.empty

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
    test <@ createRes.Request.Extra.["resource"] = "/assets" @>
    test <@ createRes.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateSuccsess @>
    test <@ Result.isOk updateRes.Result @>
    test <@ updateRes.Request.Method = HttpMethod.Post @>
    test <@ updateRes.Request.Extra.["resource"] = "/assets/update" @>
    test <@ updateRes.Request.Query.IsEmpty @>

    // Assert get
    test <@ Result.isOk getRes.Result @>
    test <@ getRes.Request.Method = HttpMethod.Post @>
    test <@ getRes.Request.Extra.["resource"] = "/assets/byids" @>
    test <@ getRes.Request.Query.IsEmpty @>
    test <@ resExternalId = Some externalIdString @>
    test <@ resName = newName @>
    test <@ metaDataOk @>

    // Assert delete
    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = HttpMethod.Post @>
    test <@ delRes.Request.Extra.["resource"] = "/assets/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}
