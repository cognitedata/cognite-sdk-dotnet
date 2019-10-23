module Tests.Integration.Files

open System
open System.Net.Http
open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open Tests
open Common
open Oryx
open FSharp.Control.Tasks.V2.ContextInsensitive

open CogniteSdk
open CogniteSdk.Files

[<Trait("resource", "files")>]
[<Fact>]
let ``List Files with limit is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let query = [ FileQuery.Limit 10 ]

    // Act
    let! res = Items.listAsync query [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get file by id is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let fileId = 2013333184649590L

    // Act
    let! res = Files.Entity.getAsync fileId ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resId = ctx'.Response.Id

    // Assert
    test <@ resId = fileId @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/2013333184649590" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get file by missing id is Error`` () = task {
    // Arrange
    let ctx = readCtx ()
    let eventId = 0L

    // Act
    let! res = Files.Entity.getAsync eventId ctx

    let err =
        match res with
        | Ok _ -> ResponseError.empty
        | Error err -> err

    // Assert
    test <@ Result.isError res @>
    test <@ err.Code = 400 @>
    test <@ err.Message.Contains "violations" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get files by ids is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let fileIds =
        [ 2013333184649590L; 2424609243916557L; 3970039428634821L ]
        |> Seq.map Identity.Id

    // Act
    let! res = Files.Retrieve.getByIdsAsync fileIds ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let ids = Seq.map (fun (d: FileReadDto) -> d.Id) dtos

    // Assert
    test <@ len = 3 @>
    test <@ Seq.forall (fun i -> Seq.contains (Identity.Id i) fileIds) ids @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/byids" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get files downloadLink by ids is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let fileIds =
        [ 2013333184649590L; 2424609243916557L; 3970039428634821L ]
        |> Seq.map Identity.Id

    // Act
    let! res = Files.DownloadLink.getDownloadLinksAsync fileIds ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let ids = Seq.map (fun (d: DownloadResponse) -> d.Identity) dtos

    // Assert
    test <@ len = 3 @>
    test <@ Seq.forall (fun i -> Seq.contains i fileIds) ids @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/downloadlink" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get files by externalIds is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let fileIds =
        [ "dotnet sdk test" ]
        |> Seq.map Identity.ExternalId

    // Act
    let! res = Files.Retrieve.getByIdsAsync fileIds ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let ids = Seq.collect (fun (d: FileReadDto) -> d.ExternalId |> optionToSeq) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "dotnet sdk test") ids @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/byids" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on AssetIds is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let filters = [
        FileFilter.AssetIds [ 5409900891232494L ]
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let assetIds = Seq.collect (fun (e: FileReadDto) -> e.AssetIds) dtos.Items

    // Assert
    test <@ len > 0 @>
    test <@ Seq.forall ((=) 5409900891232494L) assetIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on CreatedTime is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1533213749083L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1533213749099L)
    }
    let filters = [
        FileFilter.CreatedTime timerange
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let createdTimes = Seq.map (fun (e: FileReadDto) -> e.CreatedTime) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1533213749099L && t > 1533213749083L) createdTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on LastUpdatedTime is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1533213795975L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1533213795995L)
    }
    let filters = [
        FileFilter.LastUpdatedTime timerange
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let lastUpdatedTimes = Seq.map (fun (e: FileReadDto) -> e.LastUpdatedTime) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1533213795995L && t > 1533213795975L) lastUpdatedTimes @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let filters = [
        FileFilter.ExternalIdPrefix "dotnet"
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let externalIds = Seq.collect (fun (e: FileReadDto) -> e.ExternalId |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("dotnet")) externalIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on MetaData is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let filters = [
        FileFilter.MetaData (Map.ofList ["workmate_id", "474635"])
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let ms = Seq.map (fun (e: FileReadDto) -> e.MetaData) dtos.Items

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun m -> Map.tryFind "workmate_id" m = Some "474635") ms @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on Source is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let filters = [
        FileFilter.Source "Documentum"
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    let sources = Seq.collect (fun (e: FileReadDto) -> e.Source |> optionToSeq) dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "Documentum") sources @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/files/list" @>
}

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
        | Error err -> raise <| err.ToException ()

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
        | Error err -> raise <| err.ToException ()

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
        | Error err -> raise <| err.ToException ()

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
        | Error err -> raise <| err.ToException ()

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
        | Error err -> raise <| err.ToException ()

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
        | Error err -> raise <| err.ToException ()

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
        | Error err -> raise <| err.ToException ()

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
        MetaData = Map.empty
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
        | Error err -> raise <| err.ToException ()

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
        | Error err -> raise <| err.ToException ()

    test <@ Result.isOk delRes @>
    test <@ ctx''.Request.Method = HttpMethod.Post @>
    test <@ ctx''.Request.Extra.["resource"] = "/files/delete" @>
    test <@ ctx''.Request.Query.IsEmpty @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Update files is Ok`` () = task {
    // Arrange
    let wctx = writeCtx ()

    let externalIdString = Guid.NewGuid().ToString()
    let newMetadata = ([
        "key1", "value1"
        "key2", "value2"
    ]
    |> Map.ofList)
    let dto: FileWriteDto = {
        ExternalId = Some externalIdString
        Source = None
        Name = "testName"
        MimeType = None
        AssetIds = []
        SourceCreatedTime = Some 123L
        SourceModifiedTime = Some 456L
        MetaData = [
            "oldkey1", "oldvalue1"
            "oldkey2", "oldvalue2"
        ] |> Map.ofList
    }
    let externalId = Identity.ExternalId externalIdString
    let newSource = "UpdatedSource"
    let newExternalId = Guid.NewGuid().ToString()
    let newAssetId = 5409900891232494L

    // Act
    let! createRes = Files.Create.createAsync dto wctx
    let! updateRes =
        Files.Update.updateAsync [
            (externalId, [
                FileUpdate.SetSource newSource
                FileUpdate.SetAssetIds [ newAssetId ]
                FileUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
                FileUpdate.SetExternalId (Some newExternalId)
                FileUpdate.SetSourceCreatedTime 321L
                FileUpdate.SetSourceModifiedTime 654L
            ])
        ] wctx
    let! getRes = Files.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx' =
        match getRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resSource, resExternalId, resMetaData, resSourceCreatedTime, resSourceModifiedTime, resAssetIds =
        let filesResponses = getCtx'.Response
        let h = Seq.tryHead filesResponses
        match h with
        | Some fileResponse ->
            fileResponse.Source,
            fileResponse.ExternalId,
            fileResponse.MetaData,
            fileResponse.SourceCreatedTime,
            fileResponse.SourceModifiedTime,
            fileResponse.AssetIds
        | None -> None, Some "", Map.empty, None, None, [0L]

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
    test <@ createCtx'.Request.Extra.["resource"] = "/files" @>
    test <@ createCtx'.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateSuccsess @>
    test <@ updateCtx'.Request.Method = HttpMethod.Post @>
    test <@ updateCtx'.Request.Extra.["resource"] = "/files/update" @>
    test <@ updateCtx'.Request.Query.IsEmpty @>

    // Assert get
    test <@ getCtx'.Request.Method = HttpMethod.Post @>
    test <@ getCtx'.Request.Extra.["resource"] = "/files/byids" @>
    test <@ getCtx'.Request.Query.IsEmpty @>
    test <@ resExternalId = Some newExternalId @>
    test <@ resSource = Some newSource @>
    test <@ resSourceCreatedTime = Some 321L @>
    test <@ resSourceModifiedTime = Some 654L @>
    test <@ List.head resAssetIds = newAssetId  @>
    test <@ metaDataOk @>

    let newAssetId2 = 7366035474714226L

    let! updateRes2 =
        Files.Update.updateAsync [
            (Identity.ExternalId newExternalId, [
                FileUpdate.SetMetaData (Map.ofList ["newKey", "newValue"])
                FileUpdate.ChangeAssetIds ([newAssetId2], [newAssetId])
            ])
        ] wctx

    let! getRes2 = Files.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx2' =
        match getRes2 with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resMetaData2, resAssetIds2, identity =
        let filesResponses = getCtx2'.Response
        let h = Seq.tryHead filesResponses
        match h with
        | Some fileResponse ->
            fileResponse.MetaData, fileResponse.AssetIds, fileResponse.Id
        | None -> Map.empty, [0L], 0L

    // Assert get2
    test <@ getCtx2'.Request.Method = HttpMethod.Post @>
    test <@ getCtx2'.Request.Extra.["resource"] = "/files/byids" @>
    test <@ getCtx2'.Request.Query.IsEmpty @>
    test <@ Seq.head resAssetIds2 = 7366035474714226L @>
    test <@ (Map.tryFind "newKey" resMetaData2) = Some "newValue" @>

    let! updateRes3 =
        Files.Update.updateAsync [
            (Identity.Id identity, [
                FileUpdate.ClearMetaData
                FileUpdate.ClearExternalId
                FileUpdate.ClearSource
                FileUpdate.ClearSourceCreatedTime
                FileUpdate.ClearSourceModifiedTime
                FileUpdate.ClearAssetIds
            ])
        ] wctx

    let! getRes3 = Files.Retrieve.getByIdsAsync [ Identity.Id identity ] wctx
    let! delRes = Files.Delete.deleteAsync ([ Identity.Id identity]) wctx

    let getCtx3' =
        match getRes3 with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let delCtx' =
        match delRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let resExternalId2, resSource2, resMetaData3, resAssetIds3, resSourceCreatedTime2, resSourceModifiedTime2 =
        let filesResponses = getCtx3'.Response
        let h = Seq.tryHead filesResponses
        match h with
        | Some fileResponse ->
            fileResponse.ExternalId,
            fileResponse.Source,
            fileResponse.MetaData,
            fileResponse.AssetIds,
            fileResponse.SourceCreatedTime,
            fileResponse.SourceModifiedTime
        | None -> Some "", Some "", Map.empty, [0L], Some 0L, Some 0L

    // Assert get2
    test <@ getCtx3'.Request.Method = HttpMethod.Post @>
    test <@ getCtx3'.Request.Extra.["resource"] = "/files/byids" @>
    test <@ getCtx3'.Request.Query.IsEmpty @>
    test <@ resExternalId2 = None @>
    test <@ resSource2 = None @>
    test <@ resSourceCreatedTime2 = None @>
    test <@ resSourceModifiedTime2 = None @>
    test <@ List.isEmpty resAssetIds3 @>
    test <@ Map.isEmpty resMetaData3 @>

    // Assert delete
    test <@ delCtx'.Request.Method = HttpMethod.Post @>
    test <@ delCtx'.Request.Extra.["resource"] = "/files/delete" @>
    test <@ delCtx'.Request.Query.IsEmpty @>
}