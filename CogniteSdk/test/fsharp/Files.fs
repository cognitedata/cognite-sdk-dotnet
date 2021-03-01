module Tests.Integration.Files

open System
open System.Collections.Generic

open Xunit
open Swensen.Unquote

open Common
open FSharp.Control.Tasks

open CogniteSdk

[<Trait("resource", "files")>]
[<Fact>]
let ``List Files with limit is Ok`` () = task {
    // Arrange
    let query = FileQuery(Limit = Nullable 10)

    // Act
    let! res = readClient.Files.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 10 @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get file by id is Ok`` () = task {
    // Arrange
    let fileId = 230063753840368L

    // Act
    let! dto = readClient.Files.GetAsync fileId


    let resId = dto.Id

    // Assert
    test <@ resId = fileId @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get file by missing id is Error`` () = task {
    // Arrange
    let fileId = 0L

    // Act
    Assert.ThrowsAsync<ResponseException>(fun () -> readClient.Files.GetAsync fileId :> _)
    |> ignore
}
[<Trait("resource", "files")>]
[<Fact>]
let ``Get files by ids is Ok`` () = task {
    // Arrange
    let fileIds =
        [ 230063753840368L; 748012071562569L; 1296220000699223L ]

    // Act
    let! dtos = readClient.Files.RetrieveAsync fileIds

    let len = Seq.length dtos

    let ids = Seq.map (fun (d: File) -> d.Id) dtos

    // Assert
    test <@ len = 3 @>
    test <@ Seq.forall (fun i -> Seq.contains i fileIds) ids @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get files downloadLink by ids is Ok`` () = task {
    // Arrange
    let fileIds =
        [ 230063753840368L; 748012071562569L; 1296220000699223L ]

    // Act
    let! dtos = readClient.Files.DownloadAsync fileIds

    let len = Seq.length dtos

    let ids = Seq.map (fun (d: FileDownload) -> d.Id) dtos

    // Assert
    test <@ len = 3 @>
    test <@ Seq.forall (fun i -> Seq.contains i fileIds) ids @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get files by externalIds is Ok`` () = task {
    // Arrange
    let fileIds =
        [ "dotnet sdk test" ]

    // Act
    let! dtos = writeClient.Files.RetrieveAsync fileIds

    let len = Seq.length dtos

    let ids = Seq.map (fun (d: File) -> d.ExternalId) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "dotnet sdk test") ids @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on AssetIds is Ok`` () = task {
    // Arrange
    let query =
        FileQuery(
            Filter = FileFilter(AssetIds = [ 5409900891232494L ]),
            Limit = Nullable 10
        )

    // Act
    let! res = writeClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let assetIds = Seq.collect (fun (e: File) -> e.AssetIds) dtos

    // Assert
    test <@ len > 0 @>
    test <@ Seq.forall ((=) 5409900891232494L) assetIds @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on CreatedTime is Ok`` () = task {
    // Arrange
    let query =
        FileQuery(
            Filter = FileFilter(CreatedTime = TimeRange(Min=Nullable 1586949728742L, Max=Nullable 1586949728744L)),
            Limit = Nullable 10
        )

    // Act
    let! res = readClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let createdTimes = Seq.map (fun (e: File) -> e.CreatedTime) dtos

    // Assert
    test <@ len = 4 @>
    test <@ Seq.forall (fun t -> t <= 1586949728744L && t >= 1586949728742L) createdTimes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on LastUpdatedTime is Ok`` () = task {
    // Arrange
    let query =
        FileQuery(
            Filter = FileFilter(LastUpdatedTime = TimeRange(Min=Nullable 1587127447625L, Max=Nullable 1587127447627L)),
            Limit = Nullable 10
        )

    // Act
    let! res = readClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let lastUpdatedTimes = Seq.map (fun (e: File) -> e.LastUpdatedTime) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t <= 1587127447627L && t >= 1587127447625L) lastUpdatedTimes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let query =
        FileQuery(
            Filter = FileFilter(ExternalIdPrefix = "dotnet"),
            Limit = Nullable 10
        )
    // Act
    let! res = writeClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let externalIds = Seq.map (fun (e: File) -> e.ExternalId) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("dotnet")) externalIds @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on MetaData is Ok`` () = task {
    // Arrange
    let query =
        FileQuery(
            Filter = FileFilter(Metadata = (dict ["__COGNITE_PNID", "true"] |> Dictionary)),
            Limit = Nullable 10
        )

    // Act
    let! res = readClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let ms = Seq.map (fun (e: File) -> e.Metadata) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (m: Dictionary<string,string>) -> m.["__COGNITE_PNID"] = "true") ms @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on Source is Ok`` () = task {
    // Arrange
    let query =
        FileQuery(
            Filter = FileFilter(Source = "Discovery"),
            Limit = Nullable 10
        )

    // Act
    let! res = readClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let sources = Seq.map (fun (e: File) -> e.Source) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "Discovery") sources @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on Name is Ok`` () = task {
    // Arrange
    let query = FileQuery(Limit=Nullable 10, Filter=FileFilter(Name="PH-ME-P-0156-001.pdf"))

    // Act
    let! dtos = readClient.Files.ListAsync query

    let len = Seq.length dtos.Items

    let names = Seq.map (fun (e: File) -> e.Name) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "PH-ME-P-0156-001.pdf") names @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on MimeType is Ok`` () = task {
    // Arrange

    let query = FileQuery(Limit=Nullable 10, Filter=FileFilter(MimeType="application/pdf"))

    // Act
    let! dtos = readClient.Files.ListAsync query

    let len = Seq.length dtos.Items

    let mimeTypes = Seq.map (fun (e: File) -> e.MimeType) dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "application/pdf") mimeTypes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on UploadedTime is Ok`` () = task {
    // Arrange
    let timerange =
        TimeRange(
            Min = Nullable 1586949730123L,
            Max = Nullable 1586949730125L
        )
    let query = FileQuery(Limit=Nullable 10, Filter=FileFilter(UploadedTime=timerange))


    // Act
    let! dtos = readClient.Files.ListAsync query

    let len = Seq.length dtos.Items

    let uploadedTimes = Seq.map (fun (e: File) -> e.UploadedTime.Value) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t <= 1586949730125L && t >= 1586949730123L) uploadedTimes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on SourceCreatedTime is Ok`` () = task {
    // Arrange
    let timerange =
        TimeRange(
            Min = Nullable 73125430L,
            Max = Nullable 73125450L
        )
    let query = FileQuery(Limit=Nullable 10, Filter=FileFilter(SourceCreatedTime=timerange))

    // Act
    let! dtos = writeClient.Files.ListAsync query

    let len = Seq.length dtos.Items
    let len = Seq.length dtos.Items

    let sourceCreatedTimes = Seq.map (fun (e: File) -> e.SourceCreatedTime.Value) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t <= 73125450L && t >= 73125430L) sourceCreatedTimes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on SourceModifiedTime is Ok`` () = task {
    // Arrange
    let timerange =
        TimeRange(
            Min = Nullable 99304940L,
            Max = Nullable 99304960L
        )
    let query = FileQuery(Limit=Nullable 10, Filter=FileFilter(SourceModifiedTime=timerange))

    // Act
    let! dtos = writeClient.Files.ListAsync query

    let len = Seq.length dtos.Items

    let sourceModifiedTimes = Seq.map (fun (e: File) -> e.SourceModifiedTime.Value) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t <= 99304960L && t >= 99304940L) sourceModifiedTimes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on Uploaded is Ok`` () = task {
    // Arrange
    let query = FileQuery(Limit=Nullable 10, Filter=FileFilter(Uploaded=Nullable true))

    // Act
    let! dtos = readClient.Files.ListAsync query

    let len = Seq.length dtos.Items

    let uploadeds = Seq.map (fun (e: File) -> e.Uploaded) dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall id uploadeds @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Count Files matching MetaData filter is Ok`` () = task {
    // Arrange
    let query =
        FileQuery(
            Filter = FileFilter(Metadata = (dict ["__COGNITE_PNID", "true"] |> Dictionary))
        )

    // Act
    let! count = readClient.Files.AggregateAsync query
    // Assert
    test <@ count >= 10 @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Search Files on Name is Ok`` () = task {
    // Arrange
    let query = FileSearch(Search=NameSearch(Name="test"))

    // Act
    let! dtos = writeClient.Files.SearchAsync query

    let len = Seq.length dtos

    let names = Seq.map (fun (e: File) -> e.Name) dtos

    // Assert
    test <@ len > 0 @>
    test <@ Seq.forall (fun (n: string) -> n.Contains "test") names @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Create and delete files is Ok`` () = task {
    // Arrange
    let query = FileSearch(Search=NameSearch(Name="test"))

    // Act
    let externalIdString = Guid.NewGuid().ToString()
    let dto =
        FileCreate(
            Name = "testFile",
            ExternalId = externalIdString,
            SourceCreatedTime = Nullable 999L,
            SourceModifiedTime = Nullable 888L
        )

    // Act
    let! dto = writeClient.Files.UploadAsync dto
    let! delRes = writeClient.Files.DeleteAsync ([ externalIdString ])

    let resExternalId = dto.ExternalId

    // Assert
    test <@ resExternalId = externalIdString @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Update files is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString()
    let newMetadata = (dict [
        "key1", "value1"
        "key2", "value2"
    ] |> Dictionary)

    let dto =
        FileCreate(
            ExternalId = externalIdString,
            Name = "testName",
            SourceCreatedTime = Nullable 123L,
            SourceModifiedTime = Nullable 456L,
            Metadata = (dict [
                "oldkey1", "oldvalue1"
                "oldkey2", "oldvalue2"
            ] |> Dictionary)
        )
    let externalId = externalIdString
    let newSource = "UpdatedSource"
    let newExternalId = Guid.NewGuid().ToString()
    let newAssetId = 5409900891232494L

    // Act
    let! createRes = writeClient.Files.UploadAsync dto
    let! updateRes =
        writeClient.Files.UpdateAsync [
            FileUpdateItem(
                externalId = externalId,
                Update =
                    FileUpdate(
                        Source =  UpdateNullable<string>(newSource),
                        AssetIds = UpdateEnumerable<int64>([ newAssetId ]),
                        Metadata = UpdateDictionary<string>(newMetadata, [ "oldkey1" ]),
                        ExternalId = UpdateNullable<string>(newExternalId),
                        SourceCreatedTime = UpdateNullable<Nullable<int64>>(Nullable 321L),
                        SourceModifiedTime = UpdateNullable<Nullable<int64>>(Nullable 654L)
                    )
                )
            ]
    let! filesResponses = writeClient.Files.RetrieveAsync [ newExternalId ]


    let resSource, resExternalId, resMetaData, resSourceCreatedTime, resSourceModifiedTime, resAssetIds =
        let h = Seq.tryHead filesResponses
        match h with
        | Some fileResponse ->
            fileResponse.Source,
            fileResponse.ExternalId,
            fileResponse.Metadata,
            fileResponse.SourceCreatedTime,
            fileResponse.SourceModifiedTime,
            fileResponse.AssetIds
        | None -> null, "", null, Nullable 0L, Nullable 0L,  Seq.singleton 0L


    let metaDataOk =
        resMetaData.["key1"] = "value1"
        && resMetaData.["key2"] = "value2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    // Assert create


    // Assert update
    // Assert get
    test <@ resExternalId = newExternalId @>
    test <@ resSource = newSource @>
    test <@ resSourceCreatedTime = Nullable 321L @>
    test <@ resSourceModifiedTime = Nullable 654L @>
    test <@ Seq.head resAssetIds = newAssetId  @>
    test <@ metaDataOk @>

    let newAssetId2 = 7366035474714226L

    let! updateRes2 =
        writeClient.Files.UpdateAsync [
            FileUpdateItem(
                externalId = newExternalId,
                Update =
                    FileUpdate(
                        Metadata = UpdateDictionary<string>(dict ["newKey", "newValue"] |> Dictionary),
                        AssetIds = UpdateEnumerable<int64>([newAssetId2], [newAssetId])
                    )
           )
        ]

    let! filesResponses = writeClient.Files.RetrieveAsync [ newExternalId ]

    let resMetaData2, resAssetIds2, identity =
        let h = Seq.tryHead filesResponses
        match h with
        | Some fileResponse ->
            fileResponse.Metadata, fileResponse.AssetIds, fileResponse.Id
        | None -> null, Seq.singleton 0L, 0L

    // Assert get2
    test <@ Seq.head resAssetIds2 = 7366035474714226L @>
    test <@ (resMetaData2.["newKey"] ) = "newValue" @>

    let! updateRes3 =
        writeClient.Files.UpdateAsync [
            FileUpdateItem(
                id = identity,
                Update =
                    FileUpdate(
                        Metadata = UpdateDictionary(set=Dictionary()),
                        ExternalId = UpdateNullable(null),
                        Source = UpdateNullable(null),
                        SourceCreatedTime = UpdateNullable(Nullable ()),
                        SourceModifiedTime = UpdateNullable(Nullable ()),
                        AssetIds = UpdateEnumerable([])
                    )
            )]


    let! filesResponses = writeClient.Files.RetrieveAsync [ identity ]
    let! delRes = writeClient.Files.DeleteAsync [ identity ]

    let resExternalId2, resSource2, resMetaData3, resAssetIds3, resSourceCreatedTime2, resSourceModifiedTime2 =
        let h = Seq.tryHead filesResponses
        match h with
        | Some fileResponse ->
            fileResponse.ExternalId,
            fileResponse.Source,
            fileResponse.Metadata,
            fileResponse.AssetIds,
            fileResponse.SourceCreatedTime,
            fileResponse.SourceModifiedTime
        | None -> "", "", null, Seq.singleton 0L, Nullable 0L, Nullable 0L

    // Assert get2
    test <@ isNull resExternalId2 @>
    test <@ isNull resSource2 @>
    test <@ resSourceCreatedTime2 = Nullable () @>
    test <@ resSourceModifiedTime2 = Nullable () @>
    test <@ isNull resAssetIds3 @>
    test <@ Seq.isEmpty resMetaData3  @>

    // Assert delete
}

