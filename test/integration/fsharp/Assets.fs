module Tests.Integration.Assets

open System
open System.Net.Http

open Swensen.Unquote
open Xunit

open Oryx
open CogniteSdk
//open CogniteSdk.Assets

open Common
open Tests
open FSharp.Control.Tasks.V2.ContextInsensitive

[<Fact>]
let ``List assets with limit is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let query = [ Assets.AssetQuery.Limit 10 ]

    // Act
    let! res = Assets.Items.listAsync query [] ctx

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
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Get asset by id is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let assetId = 130452390632424L

    // Act
    let! res = Assets.Entity.getAsync assetId ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dto = ctx'.Response
    let resId =dto.Id

    // Assert
    test <@ resId = assetId @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/130452390632424" @>
}

[<Fact>]
let ``Get asset by missing id is Error`` () = task {
    // Arrange
    let ctx = readCtx ()
    let assetId = 0L

    // Act
    let! res = Assets.Entity.getAsync assetId ctx

    let err =
        match res with
        | Ok _ -> ResponseError.empty
        | Error (ApiError err) -> err
        | Error (Panic err) -> raise err

    // Assert
    test <@ Result.isError res @>
    test <@ err.Code = 400 @>
    test <@ err.Message.Contains "violations" @>
    test <@ Option.isSome err.RequestId @>
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
    test <@ ctx'.Request.Extra.["resource"] = "/assets/byids" @>
}

[<Fact>]
let ``Filter assets is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.RootIds [ Identity.Id 6687602007296940L ]
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

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
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on Name is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.Name "23-TE-96116-04"
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let identity = (Seq.head dtos.Items).Id

    // Assert
    test <@ len = 1 @>
    test <@ identity = 702630644612L @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.ExternalIdPrefix "odata"
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let externalids =
        dtos.Items
        |> Seq.collect (fun (dto: Assets.AssetReadDto) -> dto.ExternalId |> optionToSeq)

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith "odata") externalids @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on MetaData is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.MetaData (Map.ofList [("RES_ID", "525283")])
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let ms = Seq.map (fun (dto: Assets.AssetReadDto) -> dto.MetaData) dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (m: Map<string, string>) -> (Map.find "RES_ID" m) = "525283") ms @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on ParentIds is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.ParentIds [3117826349444493L]
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let parentIds =
        dtos.Items
        |> Seq.collect (fun (dto: Assets.AssetReadDto) -> dto.ParentId |> optionToSeq)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: int64) -> e = 3117826349444493L) parentIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on Root is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.Root true
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len = 2 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on RootIds is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.RootIds [Identity.Id 6687602007296940L]
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let rootIds =
        dtos.Items
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.RootId)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: int64) -> e = 6687602007296940L) rootIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Filter assets on Source is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        Assets.AssetQuery.Limit 10
    ]
    let filters = [
        Assets.AssetFilter.Source "cillum irure ex cupidatat dolore"
    ]

    // Act
    let! res = Assets.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let sources =
        dtos.Items
        |> Seq.collect (fun (dto: Assets.AssetReadDto) -> dto.Source |> optionToSeq)

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e = "cillum irure ex cupidatat dolore") sources @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Search assets is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        Assets.AssetSearch.Name "23"
    ]

    // Act
    let! res = Assets.Search.searchAsync 10 options [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len =Seq.length dtos

    // Assert
    test <@ len = 10 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/search" @>
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
        Assets.Search.searchAsync 10 [] [ Assets.AssetFilter.CreatedTime timerange ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let createdTime = (Seq.head dtos).CreatedTime

    // Assert
    test <@ len = 1 @>
    test <@ createdTime = 1567084348470L @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/search" @>
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
        Assets.Search.searchAsync 10 [] [ Assets.AssetFilter.LastUpdatedTime timerange] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let lastUpdatedTime = (Seq.head dtos).LastUpdatedTime

    // Assert
    test <@ len = 1 @>
    test <@ lastUpdatedTime = 1567084348470L @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Search assets on Description is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    let searches = [
        Assets.AssetSearch.Description "1STSTGGEAR THRUST"
    ]

    // Act
    let! res = Assets.Search.searchAsync 10 searches [ ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let descriptions =
        dtos
        |> Seq.collect (fun (dto: Assets.AssetReadDto) -> dto.Description |> optionToSeq)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("1STSTGGEAR THRUST")) descriptions @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Search assets on Name is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    let searches = [
        Assets.AssetSearch.Name "TE-96116"
    ]

    // Act
    let! res = Assets.Search.searchAsync 10 searches [ ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let names =
        dtos
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.Name)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("96116") || e.Contains("TE")) names @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/search" @>
}

[<Fact>]
let ``Create and delete assets is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = Guid.NewGuid().ToString();
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
    let! res = Assets.Create.createAsync [ dto ] ctx
    let! delRes = Assets.Delete.deleteAsync ([ externalId ], false) ctx

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
    test <@ ctx'.Request.Extra.["resource"] = "/assets" @>
    test <@ ctx'.Request.Query.IsEmpty @>

    test <@ delCtx'.Request.Method = HttpMethod.Post @>
    test <@ delCtx'.Request.Extra.["resource"] = "/assets/delete" @>
    test <@ delCtx'.Request.Query.IsEmpty @>
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
    let dto: Assets.AssetWriteDto = {
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
                Assets.AssetUpdate.SetName newName
                Assets.AssetUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
                Assets.AssetUpdate.SetExternalId (Some newExternalId)
            ])
        ] wctx
    let! getRes = Assets.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx' =
        match getRes with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

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
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let updateCtx' =
        match updateRes with
        | Ok ctx -> ctx
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

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
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

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
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

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
        | Error (ApiError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

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
