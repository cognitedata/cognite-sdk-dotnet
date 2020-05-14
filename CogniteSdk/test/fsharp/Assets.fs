module Tests.Integration.Assets

open System
open System.Collections.Generic

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``List assets with limit is Ok`` () = task {
    // Arrange
    let query = AssetQuery(Limit= Nullable 10)

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
    let filter = AssetFilter(RootIds = [ Identity.Create(6687602007296940L) ])
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 10 @>
}

[<Fact>]
let ``Filter assets on Name is Ok`` () = task {
    // Arrange
    let filter = AssetFilter(Name = "23-TE-96116-04")
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

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
    let filter = AssetFilter(ExternalIdPrefix = "odata")
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Assets.ListAsync query

    let len = Seq.length res.Items

    let externalids =
        res.Items
        |> Seq.map (fun (dto: Asset) -> dto.ExternalId)

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith "odata") externalids @>
}

[<Fact>]
let ``Filter assets on MetaData is Ok`` () = task {
    // Arrange
    let meta = Dictionary (dict [("RES_ID", "525283")])
    let filter = AssetFilter(Metadata = meta)
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items
    let ms = Seq.map (fun (dto: Asset) -> dto.Metadata) res.Items

    // Assert
    test <@ len = 10 @>
    test <@ ms |> Seq.forall (fun (m: Dictionary<string, string>) -> (m.Item "RES_ID") = "525283") @>
}

[<Fact>]
let ``Filter assets on ParentIds is Ok`` () = task {
    // Arrange
    let filter = AssetFilter(ParentIds = [3117826349444493L])
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items

    let parentIds =
        res.Items
        |> Seq.map (fun (dto: Asset) -> dto.ParentId)

    // Assert
    test <@ len = 10 @>
    test <@ parentIds |> Seq.forall (fun e -> e = Nullable 3117826349444493L) @>
}

[<Fact>]
let ``Filter assets on Root is Ok`` () = task {
    // Arrange
    let filter = AssetFilter(Root = Nullable true)
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 1 @>
}

[<Fact>]
let ``Filter assets on RootIds is Ok`` () = task {
    // Arrange
    let filter = AssetFilter(RootIds = [Identity 6687602007296940L])
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items
    let rootIds =
        res.Items
        |> Seq.map (fun (dto: Asset) -> dto.RootId)


    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: int64) -> e = 6687602007296940L) rootIds @>
}

[<Fact>]
let ``Filter assets on Source is Ok`` () = task {
    // Arrange
    let filter = AssetFilter(Source = "cillum irure ex cupidatat dolore")
    let query = AssetQuery(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Assets.ListAsync query

    let len = Seq.length res.Items
    let sources =
        res.Items
        |> Seq.map (fun (dto: Asset) -> dto.Source)

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e = "cillum irure ex cupidatat dolore") sources @>
}

[<Fact>]
let ``Count assets with filter is ok`` () = task {
    // Arrange
    let meta = Dictionary (dict [("RES_ID", "525283")])
    let filter = AssetFilter(Metadata = meta)
    let query = AssetQuery(Filter = filter)

    // Act
    let! count = readClient.Assets.AggregateAsync query

    // Assert
    test <@ count > 0 @>
}

[<Fact>]
let ``Search assets is Ok`` () = task {
    // Arrange
    let search = Search(Name = "23")
    let query = AssetSearch(Limit = Nullable 10, Search = search)

    // Act
    let! res = readClient.Assets.SearchAsync query
    let len = Seq.length res

    // Assert
    test <@ len = 10 @>
}

[<Fact>]
let ``Search assets on CreatedTime Ok`` () = task {
    // Arrange
    let timerange = TimeRange(Min = Nullable 1567084348460L, Max = Nullable 1567084348480L)
    let filter = AssetFilter(CreatedTime = timerange)
    let query = AssetSearch(Limit = Nullable 10, Filter = filter)

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
    let timerange = TimeRange(Min = Nullable 1567084348460L, Max = Nullable 1567084348480L)
    let filter = AssetFilter(CreatedTime = timerange)
    let query = AssetSearch(Limit = Nullable 10, Filter = filter)

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
    let search = Search(Description = "1STSTGGEAR THRUST")
    let query = AssetSearch(Limit = Nullable 10, Search = search)

    // Act
    let! res = readClient.Assets.SearchAsync query

    let len = Seq.length res
    let descriptions =
        res
        |> Seq.map (fun (dto: Asset) -> dto.Description)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("1STSTGGEAR THRUST")) descriptions @>
}

[<Fact>]
let ``Search assets on Name is Ok`` () = task {
    // Arrange
    let search = Search(Name = "TE-96116")
    let query = AssetSearch(Limit = Nullable 10, Search = search)

    // Act
    let! res = readClient.Assets.SearchAsync query

    let len = Seq.length res
    let names =
        res
        |> Seq.map (fun (dto: Asset) -> dto.Name)

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.Contains("96116") || e.Contains("TE")) names @>
}

[<Fact>]
let ``Create and delete assets is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let dto = AssetCreate(ExternalId = externalIdString, Name = "Create Assets sdk test")
    let externalId = Identity externalIdString

    // Act
    let! res = writeClient.Assets.CreateAsync [ dto ]
    let! delRes = writeClient.Assets.DeleteAsync [ externalId ]

    let resExternalId = (Seq.head res).ExternalId

    // Assert
    test <@ resExternalId = externalIdString @>
}

[<Fact>]
let ``Update assets is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let newMetadata = Dictionary(dict [
        "key1", "value1"
        "key2", "value2"
    ])

    let dto =
        AssetCreate(
            ExternalId = externalIdString,
            Name = "Create Assets sdk test",
            Description = "dotnet sdk test",
            Metadata = Dictionary(dict [
                "oldkey1", "oldvalue1"
                "oldkey2", "oldvalue2"
            ])
        )

    let externalId = externalIdString
    let newName = "UpdatedName"
    let newExternalId = Guid.NewGuid().ToString();

    let update = seq {
        AssetUpdateItem(
            externalId = externalId,
            Update = AssetUpdate(
                Name = Update(newName),
                Metadata = UpdateDictionary(add=newMetadata, remove=["oldkey1"]),
                ExternalId = UpdateNullable(newExternalId)
            )
        )
    }

    // Act
    let! createRes = writeClient.Assets.CreateAsync [ dto ]
    let! updateRes = writeClient.Assets.UpdateAsync update

    let! assetsResponses = writeClient.Assets.RetrieveAsync [ newExternalId ]

    let resName, resExternalId, resMetaData =
        let h = Seq.tryHead assetsResponses
        match h with
        | Some assetResponse -> assetResponse.Name, assetResponse.ExternalId, assetResponse.Metadata
        | None -> "", "", Dictionary ()

    let metaDataOk =
        resMetaData.["key1"] = "value1"
        && resMetaData.["key2"] = "value2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    // Assert get
    test <@ resExternalId = newExternalId @>
    test <@ resName = newName @>
    test <@ metaDataOk @>

    let newDescription = "updatedDescription"
    let newSource = "updatedSource"

    let! updateRes2 =
        writeClient.Assets.UpdateAsync [
            AssetUpdateItem(
                externalId=newExternalId,
                Update=AssetUpdate(
                    Metadata = UpdateDictionary(Dictionary(dict["newKey", "newValue"])),
                    Description = UpdateNullable(newDescription),
                    Source = UpdateNullable(newSource)
                )
            )
        ]

    let! assetsResponses = writeClient.Assets.RetrieveAsync [ newExternalId ]

    let resDescription, resSource, resMetaData2, identity =
        let h = Seq.tryHead assetsResponses
        match h with
        | Some assetResponse ->
            assetResponse.Description, assetResponse.Source, assetResponse.Metadata, assetResponse.Id
        | None -> "", "", Dictionary (), 0L

    // Assert get2
    test <@ resDescription = newDescription @>
    test <@ resSource = newSource @>
    test <@ resMetaData2.["newKey"] = "newValue" @>

    let! updateRes3 =
        writeClient.Assets.UpdateAsync [
            AssetUpdateItem(
                id = identity,
                Update = AssetUpdate(
                    Metadata = UpdateDictionary<string>(remove=["newKey"]),
                    ExternalId = UpdateNullable<string>(null),
                    Source = UpdateNullable<string>(null)
                )
            )
        ]

    let! assetsResponses = writeClient.Assets.RetrieveAsync [ identity ]
    let! delRes = writeClient.Assets.DeleteAsync [ identity]

    let resExternalId2, resSource2, resMetaData3 =
        let h = Seq.tryHead assetsResponses
        match h with
        | Some assetResponse ->
            assetResponse.ExternalId, assetResponse.Source, assetResponse.Metadata
        | None -> "", "", Dictionary ()

    // Assert get2
    test <@ isNull resExternalId2 @>
    test <@ isNull resSource2 @>
    test <@ resMetaData3.Count = 0 @>
}