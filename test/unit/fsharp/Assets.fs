module Tests.Assets

open System.IO
open System.Net.Http

open Xunit
open Swensen.Unquote

open Oryx
open CogniteSdk
open CogniteSdk.Assets
open FSharp.Control.Tasks.V2.ContextInsensitive
open System.Threading.Tasks


[<Fact>]
let ``Get asset is Ok``() = task {
    // Arrange
    let json = File.ReadAllText "Asset.json"
    let fetch = Fetch.fromJson json
    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! response = Assets.Entity.getCore 42L fetch Task.FromResult ctx

    // Assert
    test <@ Result.isOk response.Result @>
    test <@ response.Request.Method = HttpMethod.Get @>
    test <@ response.Request.Extra.["resource"] = "/assets/42" @>
    test <@ response.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get invalid asset is Error`` () = task {
    // Arrenge
    let json = File.ReadAllText "InvalidAsset.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")


    // Act
    let! response = Assets.Entity.getCore 42L fetch Task.FromResult ctx

    // Assert
    test <@ Result.isError response.Result @>
}

[<Fact>]
let ``Get asset with extra fields is Ok`` () = task {
    // Arrenge
    let json = File.ReadAllText "AssetExtra.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! response = Assets.Entity.getCore 42L fetch Task.FromResult ctx

    // Assert
    test <@ Result.isOk response.Result @>
}

[<Fact>]
let ``Get asset with missing optional fields is Ok`` () = task {
    // Arrenge

    let json = File.ReadAllText "AssetOptional.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! response = Assets.Entity.getCore 42L fetch Task.FromResult ctx

    // Assert
    test <@ Result.isOk response.Result @>
}

[<Fact>]
let ``List assets is Ok`` () = task {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    let query = [
        AssetQuery.Limit 10
        AssetQuery.Cursor "mycursor"
    ]

    let filter = [
        AssetFilter.Name "string"
        AssetFilter.Source "source"
        AssetFilter.Root false
        AssetFilter.ParentIds [42L; 43L]
    ]

    // Act
    let! res = (fetch, Task.FromResult, ctx) |||>  Assets.Items.listCore query []

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
}

[<Fact>]
let ``Create assets empty is Ok`` () = task {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! res = Assets.Create.createCore [] fetch Task.FromResult ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Create single asset is Ok`` () = task {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    let asset: AssetWriteDto = {
        Name = "myAsset"
        Description = Some "Description"
        MetaData = Map.empty
        Source = None
        ParentId = Some 42L
        ExternalId = None
        ParentExternalId = None
    }

    // Act
    let! res = Assets.Create.createCore [ asset ] fetch Task.FromResult ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets" @>
    test <@ res.Request.Query.IsEmpty @>

    match res.Result with
    | Ok assets ->
        test <@ Seq.length assets = 1 @>
    | Error error ->
        raise (Sdk.XunitException (error.ToString ()))
}

[<Fact>]
let ``Update single asset with no updates is Ok`` () = task {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! res = Assets.Update.updateCore [ Identity.Id 42L, [] ] fetch Task.FromResult ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/update" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Update single asset with is Ok`` () = task {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! res = (fetch, Task.FromResult, ctx) |||> Assets.Update.updateCore  [ (Identity.Id 42L, [
        AssetUpdate.SetName "New name"
        AssetUpdate.SetDescription (Some "New description")
        AssetUpdate.ClearSource
    ])]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/update" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Attempt searching assets`` () = task {
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")
    let options = [
        AssetSearch.Name "str"
    ]
    let filter = [
        AssetFilter.Name "string"
    ]

    let! res = (fetch, Task.FromResult, ctx) |||> Assets.Search.searchCore 100 options filter

    test <@ Result.isOk res. Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/search" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Attempt filtering assets`` () = task {
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")
    let query = [
        AssetQuery.Limit 100
    ]
    let filter = [
        AssetFilter.Name "string"
    ]

    let! res = (fetch, Task.FromResult, ctx) |||> Assets.Items.listCore query filter

    test <@ Result.isOk res. Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/assets/list" @>
    test <@ res.Request.Query.IsEmpty @>
}
