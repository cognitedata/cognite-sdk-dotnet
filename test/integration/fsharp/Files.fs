module Tests.Integration.Files

open System
open System.Net.Http
open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open Tests
open Common
open Oryx
open Oryx.Retry
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get file by id is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let fileId = 2013333184649590L

    // Act
    let! res = Files.Entity.getAsync fileId ctx

    let resId =
        match res.Result with
        | Ok dto -> dto.Id
        | Error _ -> 0L

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = fileId @>
    test <@ res.Request.Method = HttpMethod.Get @>
    test <@ res.Request.Extra.["resource"] = "/files/2013333184649590" @>
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
        match res.Result with
        | Ok _ -> ResponseError.empty
        | Error err -> err

    // Assert
    test <@ Result.isError res.Result @>
    test <@ err.Code = 400 @>
    test <@ err.Message = "getSingleFile.arg0: must be greater than or equal to 1" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    let ids =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (d: FileReadDto) -> d.Id) dtos
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 3 @>
    test <@ Seq.forall (fun i -> Seq.contains (Identity.Id i) fileIds) ids @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/byids" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Get files by externalIds is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let fileIds =
        [ "dotnet external" ]
        |> Seq.map Identity.ExternalId

    // Act
    let! res = Files.Retrieve.getByIdsAsync fileIds ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    let ids =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (d: FileReadDto) -> d.ExternalId |> optionToSeq) dtos
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "dotnet external") ids @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/byids" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let assetIds =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: FileReadDto) -> e.AssetIds) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall ((=) 5409900891232494L) assetIds @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on CreatedTime is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1568905871300L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1568905871315L)
    }
    let filters = [
        FileFilter.CreatedTime timerange
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let createdTimes =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: FileReadDto) -> e.CreatedTime) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1568905871315L && t > 1568905871300L) createdTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
}

[<Trait("resource", "files")>]
[<Fact>]
let ``Filter Files on LastUpdatedTime is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let options = [
        FileQuery.Limit 10
    ]
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1568905871300L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1568905871315L)
    }
    let filters = [
        FileFilter.LastUpdatedTime timerange
    ]

    // Act
    let! res = Files.Items.listAsync options filters ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let lastUpdatedTimes =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: FileReadDto) -> e.CreatedTime) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1568905871315L && t > 1568905871300L) lastUpdatedTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let externalIds =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: FileReadDto) -> e.ExternalId |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("dotnet")) externalIds @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let ms =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: FileReadDto) -> e.MetaData) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun m -> Map.tryFind "workmate_id" m = Some "474635") ms @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let sources =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: FileReadDto) -> e.Source |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall ((=) "Documentum") sources @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let names =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: FileReadDto) -> e.Name) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "PH-ME-P-0003-001") names @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let mimeTypes =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: FileReadDto) -> e.MimeType |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 7 @>
    test <@ Seq.forall ((=) "pdf") mimeTypes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let uploadedTimes =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: FileReadDto) -> e.UploadedTime |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 1533213278689L && t > 1533213278669L) uploadedTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let sourceCreatedTimes =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: FileReadDto) -> e.SourceCreatedTime |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 73125450L && t > 73125430L) sourceCreatedTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let sourceModifiedTimes =
        match res.Result with
        | Ok dtos ->
            Seq.collect (fun (e: FileReadDto) -> e.SourceModifiedTime |> optionToSeq) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun t -> t < 99304960L && t > 99304940L) sourceModifiedTimes @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    let uploadeds =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: FileReadDto) -> e.Uploaded) dtos.Items
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ Seq.forall id uploadeds @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/list" @>
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

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    let names =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (e: FileReadDto) -> e.Name) dtos
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ Seq.forall (fun (n: string) -> n.Contains "test") names @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files/search" @>
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
    let resExternalId =
        match res.Result with
        | Ok filesResponse -> filesResponse.ExternalId
        | Error _ -> None

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resExternalId = Some externalIdString @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/files" @>
    test <@ res.Request.Query.IsEmpty @>

    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = HttpMethod.Post @>
    test <@ delRes.Request.Extra.["resource"] = "/files/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}