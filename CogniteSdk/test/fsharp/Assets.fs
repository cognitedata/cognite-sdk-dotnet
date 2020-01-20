module Tests.Integration.Assets

open System
open System.Collections.Generic

open Swensen.Unquote
open Xunit

open CogniteSdk
open Common
open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx

[<Fact>]
let ``List assets with limit is Ok`` () = task {
    // Arrange
    let query = Assets.AssetQueryDto().WithLimit(10)

    // // Act
    let! res = readClient.Assets.ListAsync(query)
    let len = Seq.length res.Items

    // // Assert
    test <@ 10 = 10 @>
}

[<Fact>]
let ``Get asset by id is Ok`` () = task {
    // Arrange
    let assetId = 130452390632424L

    // Act
    let! res = readClient.Assets.GetAsync assetId

    let resId = res.Id

    // Assert
    test <@ resId = assetId @>
}

[<Fact>]
let ``Get asset by missing id is Error`` () = task {
    // Arrange
    let assetId = 0L

    // Act
    let! res =
        task {
            try
                let! a = readClient.Assets.GetAsync assetId
                return Ok a
            with
            | :? ResponseException as e -> return Error e
        }

    let err = Result.getError res

    // Assert
    test <@ Result.isError res @>
    test <@ err.Code = 400 @>
    test <@ err.Message.Contains "violations" @>
    test <@ not (isNull err.RequestId) @>
}

[<Fact>]
let ``Get asset by ids is Ok`` () = task {
    // Arrange
    let assetIds =
        [ 130452390632424L; 126847700303897L; 124419735577853L ]

    // Act
    let! res = readClient.Assets.RetrieveAsync assetIds

    let len = Seq.length res

    // Assert
    test <@ len = 3 @>
}

[<Fact>]
let ``Filter assets is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(RootIds = [ Identity.Create(6687602007296940L) ])
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 10 @>
}

[<Fact>]
let ``Filter assets on Name is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(Name = "23-TE-96116-04")
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items
    let identity = (Seq.head res.Items).Id

    // Assert
    test <@ len = 1 @>
    test <@ identity = 702630644612L @>
}

[<Fact>]
let ``Filter assets on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(ExternalIdPrefix = "odata")
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Assets.ListAsync query

    let len = Seq.length res.Items

    let externalids =
        res.Items
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.ExternalId)

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith "odata") externalids @>
}

[<Fact>]
let ``Filter assets on MetaData is Ok`` () = task {
    // Arrange
    let meta = Map.ofList [("RES_ID", "525283")]
    let filter = Assets.AssetFilterDto(Metadata = meta)
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items
    let ms = Seq.map (fun (dto: Assets.AssetReadDto) -> dto.Metadata) res.Items

    // Assert
    test <@ len = 10 @>
    test <@ ms |> Seq.forall (fun (m: IDictionary<string, string>) -> (m.Item "RES_ID") = "525283") @>
}

[<Fact>]
let ``Filter assets on ParentIds is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(ParentIds = [3117826349444493L])
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items

    let parentIds =
        res.Items
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.ParentId)

    // Assert
    test <@ len = 10 @>
    test <@ parentIds |> Seq.forall (fun e -> e = Nullable 3117826349444493L) @>
}

[<Fact>]
let ``Filter assets on Root is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(Root = Nullable true)
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 2 @>
}

[<Fact>]
let ``Filter assets on RootIds is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(RootIds = [Identity 6687602007296940L])
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items
    let rootIds =
        res.Items
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.RootId)


    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: int64) -> e = 6687602007296940L) rootIds @>
}

[<Fact>]
let ``Filter assets on Source is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(Source = "cillum irure ex cupidatat dolore")
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Assets.ListAsync query

    let len = Seq.length res.Items
    let sources =
        res.Items
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.Source)

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e = "cillum irure ex cupidatat dolore") sources @>
}

[<Fact>]
let ``Search assets is Ok`` () = task {
    // Arrange
    let search = SearchDto(Name = "23")
    let query = SearchQueryDto(Limit = Nullable 10, Search = search)

    // Act
    let! res = readClient.Assets.SearchAsync query
    let len = Seq.length res

    // Assert
    test <@ len = 10 @>
}

[<Fact>]
let ``Search assets on CreatedTime Ok`` () = task {
    // Arrange
    let timerange = TimeRange(Min = 1567084348460L, Max = 1567084348480L)
    let filter = Assets.AssetFilterDto(CreatedTime = timerange)
    let query = SearchQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Assets.SearchAsync query

    let len = Seq.length res
    let createdTime = (Seq.head res).CreatedTime

    // Assert
    test <@ len = 1 @>
    test <@ createdTime = 1567084348470L @>
}

[<Fact>]
let ``Search assets on LastUpdatedTime Ok`` () = task {

    // Arrange
    let timerange = TimeRange(Min = 1567084348460L, Max = 1567084348480L)
    let filter = Assets.AssetFilterDto(CreatedTime = timerange)
    let query = SearchQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Assets.SearchAsync query

    let len = Seq.length res
    let lastUpdatedTime = (Seq.head res).LastUpdatedTime

    // Assert
    test <@ len = 1 @>
    test <@ lastUpdatedTime = 1567084348470L @>
}

[<Fact>]
let ``Search assets on Description is Ok`` () = task {
    // Arrange
    let search = SearchDto(Description = "1STSTGGEAR THRUST")
    let query = SearchQueryDto(Limit = Nullable 10, Search = search)

    // Act
    let! res = readClient.Assets.SearchAsync query

    let len = Seq.length res
    let descriptions =
        res
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.Description)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("1STSTGGEAR THRUST")) descriptions @>
}

[<Fact>]
let ``Search assets on Name is Ok`` () = task {
    // Arrange
    let search = SearchDto(Name = "TE-96116")
    let query = SearchQueryDto(Limit = Nullable 10, Search = search)

    // Act
    let! res = readClient.Assets.SearchAsync query

    let len = Seq.length res
    let names =
        res
        |> Seq.map (fun (dto: Assets.AssetReadDto) -> dto.Name)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("96116") || e.Contains("TE")) names @>
}

[<Fact>]
let ``Create and delete assets is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let dto = Assets.AssetWriteDto(ExternalId = externalIdString, Name = "Create Assets sdk test")
    let externalId = Identity externalIdString

    // Act
    let! res = writeClient.Assets.CreateAsync [ dto ]
    let! delRes = writeClient.Assets.DeleteAsync [ externalId ]

    let resExternalId = (Seq.head res).ExternalId

    // Assert
    test <@ resExternalId = externalIdString @>
}

// [<Fact>]
// let ``Update assets is Ok`` () = task {
//     // Arrange
//     let externalIdString = Guid.NewGuid().ToString();
//     let newMetadata = ([
//         "key1", "value1"
//         "key2", "value2"
//     ]
//     |> Map.ofList)
//     let dto = Assets.AssetWriteDto(
//         ExternalId = externalIdString,
//         Name = "Create Assets sdk test",
//         Description = "dotnet sdk test",
//         Metadata = dict [
//             "oldkey1", "oldvalue1"
//             "oldkey2", "oldvalue2"
//         ]
//     )
//     let externalId = externalIdString
//     let newName = "UpdatedName"
//     let newExternalId = "updatedExternalId"

//     let update = List<UpdateByExternalId<Assets.AssetUpdateDto>>([
//         UpdateByExternalId<Assets.AssetUpdateDto>(
//             ExternalId = externalId,
//             Update = Assets.AssetUpdateDto(
//                             Name = SetUpdate<string>(Set = newName),
//                             Metadata = SetUpdate<IDictionary<string, string>>(Set = newMetadata),
//                             ExternalId = SetUpdate<string>(Set = newExternalId)
//                         )
//         )
//     ] |> Seq.ofList) :> IEnumerable<UpdateByExternalId<Assets.AssetUpdateDto>>

//     // Act
//     let! createRes = writeClient.Assets.CreateAsync [ dto ]
//     let! updateRes = writeClient.Assets.UpdateAsync (update :> IEnumerable<UpdateItemType<Assets.AssetUpdateDto>>)


//         // ]
//             // (externalId, [
//             //      newName
//             //     Assets.AssetUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
//             //     Assets.AssetUpdate.SetExternalId (Some newExternalId)
//             // ])

//     let! getRes = Assets.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

//     let resName, resExternalId, resMetaData =
//         let assetsResponses = getCtx'.Response
//         let h = Seq.tryHead assetsResponses
//         match h with
//         | Some assetResponse -> assetResponse.Name, assetResponse.ExternalId, assetResponse.MetaData
//         | None -> "", Some "", Map.empty

//     let updateSuccsess = Result.isOk updateRes

//     let metaDataOk =
//         (Map.tryFind "key1" resMetaData) = Some "value1"
//         && (Map.tryFind "key2" resMetaData) = Some "value2"
//         && resMetaData.ContainsKey "oldkey2"
//         && not (resMetaData.ContainsKey "oldkey1")

//     // Assert update
//     test <@ updateSuccsess @>

//     // Assert get
//     test <@ resExternalId = Some "updatedExternalId" @>
//     test <@ resName = newName @>
//     test <@ metaDataOk @>

//     let newDescription = "updatedDescription"
//     let newSource = "updatedSource"

//     let! updateRes2 =
//         Assets.Update.updateAsync [
//             (Identity.ExternalId newExternalId, [
//                 Assets.AssetUpdate.SetMetaData (Map.ofList ["newKey", "newValue"])
//                 Assets.AssetUpdate.SetDescription (Some newDescription)
//                 Assets.AssetUpdate.SetSource newSource
//             ])
//         ] wctx

//     let! getRes2 = Assets.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

//     let resDescription, resSource, resMetaData2, identity =
//         let assetsResponses = getCtx2'.Response
//         let h = Seq.tryHead assetsResponses
//         match h with
//         | Some assetResponse ->
//             assetResponse.Description, assetResponse.Source, assetResponse.MetaData, assetResponse.Id
//         | None -> Some "", Some "", Map.empty, 0L

//     // Assert get2
//     test <@ resDescription = Some newDescription @>
//     test <@ resSource = Some newSource @>
//     test <@ (Map.tryFind "newKey" resMetaData2) = Some "newValue" @>

//     let! updateRes3 =
//         Assets.Update.updateAsync [
//             (Identity.Id identity, [
//                 Assets.AssetUpdate.ChangeMetaData (Map.empty, ["newKey"])
//                 Assets.AssetUpdate.ClearExternalId
//                 Assets.AssetUpdate.ClearSource
//             ])
//         ] wctx

//     let! getRes3 = Assets.Retrieve.getByIdsAsync [ Identity.Id identity ] wctx
//     let! delRes = Assets.Delete.deleteAsync ([ Identity.Id identity], false) wctx

//     let resExternalId2, resSource2, resMetaData3 =
//         let assetsResponses = getCtx3'.Response
//         let h = Seq.tryHead assetsResponses
//         match h with
//         | Some assetResponse ->
//             assetResponse.ExternalId, assetResponse.Source, assetResponse.MetaData
//         | None -> Some "", Some "", Map.empty

//     // Assert get2
//     test <@ resExternalId2 = None @>
//     test <@ resSource2 = None @>
//     test <@ Map.isEmpty resMetaData3 @>
// }