module Tests.Integration.Assets

open Xunit
open Swensen.Unquote

open Fusion
open Tests
open Common

[<Fact>]
let ``Get assets with limit is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [ GetAssets.Option.Limit 10 ]

    // Act
    let! res = getAssetsAsync options ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = RequestMethod.GET @>
    test <@ res.Request.Resource = "/assets" @>
}

[<Fact>]
let ``Get asset by id is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let assetId = 130452390632424L

    // Act
    let! res = getAssetAsync assetId ctx

    let resId =
        match res.Result with
        | Ok dto -> dto.Id
        | Error _ -> 0L

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = assetId @>
    test <@ res.Request.Method = RequestMethod.GET @>
    test <@ res.Request.Resource = "/assets/130452390632424" @>
}


[<Fact>]
let ``Get asset by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let assetIds =
        [ 130452390632424L; 126847700303897L; 124419735577853L ]
        |> Seq.map Identity.Id

    // Act
    let! res = getAssetsByIdsAsync assetIds ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 3 @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/assets/byids" @>
}

[<Fact>]
let ``Search assets is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        SearchAssets.Option.Name "23"
    ]

    // Act
    let! res = searchAssetsAsync 10 options [] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/assets/search" @>
}

[<Fact>]
let ``Filter assets is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FilterAssets.Option.Limit 10
    ]
    let filters = [
        FilterAssets.Filter.RootIds [ Identity.Id 6687602007296940L ]
    ]

    // Act
    let! res = filterAssetsAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/assets/list" @>
}

[<Fact>]
let ``Create and delete assets is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = "createDeleteTestAssets"
    let dto: Assets.AssetWriteDto = {
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
    let! res = createAssetsAsync [ dto ] ctx
    let! delRes = deleteAssetsAsync ([ externalId ], false) ctx
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
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/assets" @>
    test <@ res.Request.Query.IsEmpty @>

    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = RequestMethod.POST @>
    test <@ delRes.Request.Resource = "/assets/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Update assets is Ok`` () = async {
    // Arrange
    let wctx = writeCtx ()

    let externalIdString = "createDeleteTestAssets"
    let dto: Assets.AssetWriteDto = {
        ExternalId = Some externalIdString
        Name = "Create Assets sdk test"
        ParentId = None
        Description = Some "dotnet sdk test"
        MetaData = Map.empty
        Source = None
        ParentExternalId = None
    }
    let externalId = Identity.ExternalId externalIdString
    let newName = "UpdatedName"
    // Act
    let! createRes = createAssetsAsync [ dto ] wctx
    let! updateRes =
        updateAssetsAsync [
            (externalId, [ UpdateAssets.Option.SetName newName ])
        ] wctx
    let! getRes = getAssetsByIdsAsync [ externalId ] wctx
    let! delRes = deleteAssetsAsync ([ externalId ], false) wctx

    let resName, resExternalId =
        match getRes.Result with
        | Ok assetsResponses ->
            let h = Seq.tryHead assetsResponses
            match h with
            | Some assetResponse -> assetResponse.Name, assetResponse.ExternalId
            | None -> "", Some ""
        | Error _ -> "", Some ""

    let updateSuccsess =
        match updateRes.Result with
        | Ok res -> res
        | Error _ -> false

    // Assert create
    test <@ Result.isOk createRes.Result @>
    test <@ createRes.Request.Method = RequestMethod.POST @>
    test <@ createRes.Request.Resource = "/assets" @>
    test <@ createRes.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateSuccsess @>
    test <@ Result.isOk updateRes.Result @>
    test <@ updateRes.Request.Method = RequestMethod.POST @>
    test <@ updateRes.Request.Resource = "/assets/update" @>
    test <@ updateRes.Request.Query.IsEmpty @>

    // Assert get
    test <@ Result.isOk getRes.Result @>
    test <@ getRes.Request.Method = RequestMethod.POST @>
    test <@ getRes.Request.Resource = "/assets/byids" @>
    test <@ getRes.Request.Query.IsEmpty @>
    test <@ resExternalId = Some externalIdString @>
    test <@ resName = newName @>

    // Assert delete
    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = RequestMethod.POST @>
    test <@ delRes.Request.Resource = "/assets/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}