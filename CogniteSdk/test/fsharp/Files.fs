module Tests.Integration.Files

open System
open System.Collections.Generic

open Xunit
open Swensen.Unquote

open Common
open FSharp.Control.Tasks.V2.ContextInsensitive

open CogniteSdk
open CogniteSdk.Files

[<Trait("resource", "files")>]
[<Fact>]
let ``List Files with limit is Ok`` () = task {
    // Arrange
    let query = FileQueryDto(Limit = Nullable 10)

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
    let fileId = 2013333184649590L

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
        [ 2013333184649590L; 2424609243916557L; 3970039428634821L ]
        
    // Act
    let! dtos = readClient.Files.RetrieveAsync fileIds

    let len = Seq.length dtos

    let ids = Seq.map (fun (d: FileReadDto) -> d.Id) dtos

    // Assert
    test <@ len = 3 @>
    test <@ Seq.forall (fun i -> Seq.contains i fileIds) ids @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get files downloadLink by ids is Ok`` () = task {
    // Arrange
    let fileIds =
        [ 2013333184649590L; 2424609243916557L; 3970039428634821L ]
        
    // Act
    let! dtos = readClient.Files.DownloadAsync fileIds

    let len = Seq.length dtos

    let ids = Seq.map (fun (d: FileDownloadDto) -> d.Id) dtos

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

    let ids = Seq.map (fun (d: FileReadDto) -> d.ExternalId) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "dotnet sdk test") ids @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on AssetIds is Ok`` () = task {
    // Arrange
    let query = 
        FileQueryDto(
            Filter = FileFilterDto(AssetIds = [ 5409900891232494L ]),
            Limit = Nullable 10
        )

    // Act
    let! res = writeClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let assetIds = Seq.collect (fun (e: FileReadDto) -> e.AssetIds) dtos

    // Assert
    test <@ len > 0 @>
    test <@ Seq.forall ((=) 5409900891232494L) assetIds @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on CreatedTime is Ok`` () = task {
    // Arrange
    let query = 
        FileQueryDto(
            Filter = FileFilterDto(CreatedTime = TimeRange(Min=1533213749083L, Max=1533213749099L)),
            Limit = Nullable 10
        )
    
    // Act
    let! res = readClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let createdTimes = Seq.map (fun (e: FileReadDto) -> e.CreatedTime) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1533213749099L && t > 1533213749083L) createdTimes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on LastUpdatedTime is Ok`` () = task {
    // Arrange
    let query = 
        FileQueryDto(
            Filter = FileFilterDto(LastUpdatedTime = TimeRange(Min=1533213795975L, Max=1533213795995L)),
            Limit = Nullable 10
        )
    
    // Act
    let! res = readClient.Files.ListAsync query
    
    let dtos = res.Items
    let len = Seq.length dtos

    let lastUpdatedTimes = Seq.map (fun (e: FileReadDto) -> e.LastUpdatedTime) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1533213795995L && t > 1533213795975L) lastUpdatedTimes @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let query = 
        FileQueryDto(
            Filter = FileFilterDto(ExternalIdPrefix = "dotnet"),
            Limit = Nullable 10
        )
    // Act
    let! res = writeClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let externalIds = Seq.map (fun (e: FileReadDto) -> e.ExternalId) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("dotnet")) externalIds @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on MetaData is Ok`` () = task {
    // Arrange
    let query = 
        FileQueryDto(
            Filter = FileFilterDto(Metadata = (dict ["workmate_id", "474635"] |> Dictionary)),
            Limit = Nullable 10
        )

    // Act
    let! res = readClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let ms = Seq.map (fun (e: FileReadDto) -> e.Metadata) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (m: Dictionary<string,string>) -> m.["workmate_id"] = "474635") ms @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on Source is Ok`` () = task {
    // Arrange
    let query = 
        FileQueryDto(
            Filter = FileFilterDto(Source = "Documentum"),
            Limit = Nullable 10
        )

    // Act
    let! res = readClient.Files.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    let sources = Seq.map (fun (e: FileReadDto) -> e.Source) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "Documentum") sources @>
}
(*
[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on Name is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let filters = [
        FileFilter.Name "PH-ME-P-0003-001"
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let names = Seq.map (fun (e: FileReadDto) -> e.Name) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "PH-ME-P-0003-001") names @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on MimeType is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let filters = [
        FileFilter.MimeType "pdf"
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let mimeTypes = Seq.collect (fun (e: FileReadDto) -> e.MimeType |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 7 @>
    test <@ Seq.forall ((=) "pdf") mimeTypes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on UploadedTime is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1533213278669L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1533213278689L)
    }
    let filters = [
        FileFilter.UploadedTime timerange
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let uploadedTimes = Seq.collect (fun (e: FileReadDto) -> e.UploadedTime |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1533213278689L && t > 1533213278669L) uploadedTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on SourceCreatedTime is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(73125430L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(73125450L)
    }
    let filters = [
        FileFilter.SourceCreatedTime timerange
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let sourceCreatedTimes = Seq.collect (fun (e: FileReadDto) -> e.SourceCreatedTime |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 73125450L && t > 73125430L) sourceCreatedTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on SourceModifiedTime is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(99304940L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(99304960L)
    }
    let filters = [
        FileFilter.SourceModifiedTime timerange
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let sourceModifiedTimes = Seq.collect (fun (e: FileReadDto) -> e.SourceModifiedTime |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 99304960L && t > 99304940L) sourceModifiedTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on Uploaded is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let filters = [
        FileFilter.Uploaded true
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let uploadeds = Seq.map (fun (e: FileReadDto) -> e.Uploaded) dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall id uploadeds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Search Files on Name is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let searches = [
        FileSearch.Name "test"
    ]

    // Act
    let! res = Files.Search.searchAsync 10 searches [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let names = Seq.map (fun (e: FileReadDto) -> e.Name) dtos

    // Assert
    test <@ len > 0 @>
    test <@ Seq.forall (fun (n: string) -> n.Contains "test") names @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/search" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Create and delete files is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = Guid.NewGuid().ToString()
    let dto: Files.FileWriteDto = {
        Name = "testFile"
        ExternalId = Some externalIdString
        MimeType = None
        Metadata = Map.empty
        AssetIds = Seq.empty
        Source = None
        SourceCreatedTime = Some 999L
        SourceModifiedTime = Some 888L
    }
    let externalId = Identity.ExternalId externalIdString

    // Act
    let! res = Files.Create.createAsync dto ctx
    let! delRes = Files.Delete.deleteAsync ([ externalId ]) ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response

    let filesResponse = ctx'.Response
    let resExternalId = filesResponse.ExternalId

    // Assert
    test <@ resExternalId = Some externalIdString @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files" @>
    test <@ ctx'.Request.Query.IsEmpty @>

    let ctx'' =
        match delRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    test <@ Result.isOk delRes @>
    test <@ ctx''.Request.Method = HttpMethod.Post @>
    test <@ ctx''.Request.Extra.["resource"] = "/files/delete" @>
    test <@ ctx''.Request.Query.IsEmpty @>
}

*)

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
        FileWriteDto(
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
                ExternalId = externalId,
                Update = 
                    FileUpdateDto(
                        Source =  Update<string>(newSource),
                        AssetIds = SequenceUpdate<int64>([ newAssetId ]),
                        Metadata = DictUpdate<string>(newMetadata, [ "oldkey1" ]),
                        ExternalId = Update<string>(newExternalId),
                        SourceCreatedTime = Update<Nullable<int64>>(Nullable 321L),
                        SourceModifiedTime = Update<Nullable<int64>>(Nullable 654L)
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
        | None -> null, "", null, 0L, 0L,  Seq.singleton 0L

    
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
    test <@ resSourceCreatedTime = 321L @>
    test <@ resSourceModifiedTime = 654L @>
    test <@ Seq.head resAssetIds = newAssetId  @>
    test <@ metaDataOk @>

    let newAssetId2 = 7366035474714226L

    let! updateRes2 =
        writeClient.Files.UpdateAsync [
            FileUpdateItem(
                ExternalId = newExternalId,
                Update = 
                    FileUpdateDto(
                        Metadata = DictUpdate<string>(dict ["newKey", "newValue"] |> Dictionary),
                        AssetIds = SequenceUpdate<int64>([newAssetId2], [newAssetId])
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
                Id = Nullable identity,
                Update = 
                    FileUpdateDto(
                        Metadata = DictUpdate(set=Dictionary()),
                        ExternalId = Update(null),
                        Source = Update(null),
                        SourceCreatedTime = Update(Nullable ()),
                        SourceModifiedTime = Update(Nullable ()),
                        AssetIds = SequenceUpdate([])
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
        | None -> "", "", null, Seq.singleton 0L, 0L, 0L

    // Assert get2
    test <@ resExternalId2 = null @>
    test <@ resSource2 = null @>
    test <@ resSourceCreatedTime2 = 0L @>
    test <@ resSourceModifiedTime2 = 0L @>
    test <@ resAssetIds3 = null @>
    test <@ Seq.isEmpty resMetaData3  @>

    // Assert delete
}

