module Tests.Integration.Assets

open System
open System.Net.Http

open Swensen.Unquote
open Xunit

open Oryx
open CogniteSdk
open CogniteSdk.Assets

open Common
open Tests
open FSharp.Control.Tasks.V2.ContextInsensitive

[<Fact>]
let ``List assets with limit is Ok`` () = task {
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
let ``Get asset by id is Ok`` () = task {
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
let ``Get asset by missing id is Error`` () = task {
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
let ``Get asset by ids is Ok`` () = task {
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
let ``Filter assets is Ok`` () = task {
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
let ``Filter assets on Name is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.Name "23-TE-96116-04"
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let identity =
        match res.Result with
        | Ok dtos -> (Seq.head dtos.Items).Id
        | Error _ -> 0L

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ identity = 702630644612L @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.ExternalIdPrefix "odata"
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let externalids =
        match res.Result with
        | Ok dtos ->
            dtos.Items
            |> Seq.collect (fun (dto: AssetReadDto) -> dto.ExternalId |> optionToSeq)
        | Error _ -> Seq.ofList [ "" ]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith "odata") externalids @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on MetaData is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.MetaData (Map.ofList [("RES_ID", "525283")])
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let ms =
        match res.Result with
        | Ok dtos -> Seq.map (fun (dto: AssetReadDto) -> dto.MetaData) dtos.Items
        | Error _ -> Seq.ofList [Map.empty]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall (fun (m: Map<string, string>) -> (Map.find "RES_ID" m) = "525283") ms @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on ParentIds is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.ParentIds [3117826349444493L]
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let parentIds =
        match res.Result with
        | Ok dtos ->
            dtos.Items
            |> Seq.collect (fun (dto: AssetReadDto) -> dto.ParentId |> optionToSeq)
        | Error _ -> Seq.ofList [ 0L ]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: int64) -> e = 3117826349444493L) parentIds @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on Root is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.Root true
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 2 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on RootIds is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.RootIds [Identity.Id 6687602007296940L]
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let rootIds =
        match res.Result with
        | Ok dtos ->
            dtos.Items
            |> Seq.map (fun (dto: AssetReadDto) -> dto.RootId)
        | Error _ -> Seq.ofList [ 0L ]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: int64) -> e = 6687602007296940L) rootIds @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on Source is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        AssetQuery.Limit 10
    ]
    let filters = [
        AssetFilter.Source "cillum irure ex cupidatat dolore"
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let sources =
        match res.Result with
        | Ok dtos ->
            dtos.Items
            |> Seq.collect (fun (dto: AssetReadDto) -> dto.Source |> optionToSeq)
        | Error _ -> Seq.ofList [ "" ]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e = "cillum irure ex cupidatat dolore") sources @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Search assets is Ok`` () = task {
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
let ``Search assets on CreatedTime Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1567084348460L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1567084348480L)
    }

    // Act
    let! res =
        Assets.Search.searchAsync 10 [] [ AssetFilter.CreatedTime timerange ] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    let createdTime =
        match res.Result with
        | Ok dtos -> (Seq.head dtos).CreatedTime
        | Error _ -> 0L

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ createdTime = 1567084348470L @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Search assets on LastUpdatedTime Ok`` () = task {

    // Arrange
    let ctx = writeCtx ()
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1567084348460L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1567084348480L)
    }
    // Act
    let! res =
        Assets.Search.searchAsync 10 [] [ AssetFilter.LastUpdatedTime timerange] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    let lastUpdatedTime =
        match res.Result with
        | Ok dtos -> (Seq.head dtos).LastUpdatedTime
        | Error _ -> 0L

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ lastUpdatedTime = 1567084348470L @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Search assets on Description is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    let searches = [
        AssetSearch.Description "1STSTGGEAR THRUST"
    ]

    // Act
    let! res = Assets.Search.searchAsync 10 searches [ ] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    let descriptions =
        match res.Result with
        | Ok dtos ->
            dtos
            |> Seq.collect (fun (dto: AssetReadDto) -> dto.Description |> optionToSeq)
        | Error _ -> Seq.ofList [ "" ]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("1STSTGGEAR THRUST")) descriptions @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Search assets on Name is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    let searches = [
        AssetSearch.Name "TE-96116"
    ]

    // Act
    let! res = Assets.Search.searchAsync 10 searches [ ] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    let names =
        match res.Result with
        | Ok dtos ->
            dtos
            |> Seq.map (fun (dto: AssetReadDto) -> dto.Name)
        | Error _ -> Seq.ofList [ "" ]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("96116") || e.Contains("TE")) names @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Create and delete assets is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = Guid.NewGuid().ToString();
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

[<Trait("a", "a")>]
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
    let newExternalId = "updatedExternalId"

    // Act
    let! createRes = Assets.Create.createAsync [ dto ] wctx
    let! updateRes =
        Assets.Update.updateAsync [
            (externalId, [
                AssetUpdate.SetName newName
                AssetUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
                AssetUpdate.SetExternalId (Some newExternalId)
            ])
        ] wctx
    let! getRes = Assets.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

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
        (Map.tryFind "key1" resMetaData) = Some "value1"
        && (Map.tryFind "key2" resMetaData) = Some "value2"
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
    test <@ resExternalId = Some "updatedExternalId" @>
    test <@ resName = newName @>
    test <@ metaDataOk @>

    let newDescription = "updatedDescription"
    let newSource = "updatedSource"

    let! updateRes2 =
        Assets.Update.updateAsync [
            (Identity.ExternalId newExternalId, [
                AssetUpdate.SetMetaData (Map.ofList ["newKey", "newValue"])
                AssetUpdate.SetDescription (Some newDescription)
                AssetUpdate.SetSource newSource
            ])
        ] wctx

    let! getRes2 = Assets.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let resDescription, resSource, resMetaData2, identity =
        match getRes2.Result with
        | Ok assetsResponses ->
            let h = Seq.tryHead assetsResponses
            match h with
            | Some assetResponse ->
                assetResponse.Description, assetResponse.Source, assetResponse.MetaData, assetResponse.Id
            | None -> Some "", Some "", Map.empty, 0L
        | Error _ -> Some "", Some "", Map.empty, 0L

    // Assert get2
    test <@ Result.isOk getRes2.Result @>
    test <@ getRes2.Request.Method = HttpMethod.Post @>
    test <@ getRes2.Request.Extra.["resource"] = "/assets/byids" @>
    test <@ getRes2.Request.Query.IsEmpty @>
    test <@ resDescription = Some newDescription @>
    test <@ resSource = Some newSource @>
    test <@ (Map.tryFind "newKey" resMetaData2) = Some "newValue" @>

    let! updateRes3 =
        Assets.Update.updateAsync [
            (Identity.Id identity, [
                AssetUpdate.ChangeMetaData (Map.empty, ["newKey"])
                AssetUpdate.ClearExternalId
                AssetUpdate.ClearSource
            ])
        ] wctx

    let! getRes3 = Assets.Retrieve.getByIdsAsync [ Identity.Id identity ] wctx
    let! delRes = Assets.Delete.deleteAsync ([ Identity.Id identity], false) wctx

    let resExternalId2, resSource2, resMetaData3 =
        match getRes3.Result with
        | Ok assetsResponses ->
            let h = Seq.tryHead assetsResponses
            match h with
            | Some assetResponse ->
                assetResponse.ExternalId, assetResponse.Source, assetResponse.MetaData
            | None -> Some "", Some "", Map.empty
        | Error _ -> Some "", Some "", Map.empty

    // Assert get2
    test <@ Result.isOk getRes2.Result @>
    test <@ getRes2.Request.Method = HttpMethod.Post @>
    test <@ getRes2.Request.Extra.["resource"] = "/assets/byids" @>
    test <@ getRes2.Request.Query.IsEmpty @>
    test <@ resExternalId2 = None @>
    test <@ resSource2 = None @>
    test <@ Map.isEmpty resMetaData3 @>

    // Assert delete
    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = HttpMethod.Post @>
    test <@ delRes.Request.Extra.["resource"] = "/assets/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}
