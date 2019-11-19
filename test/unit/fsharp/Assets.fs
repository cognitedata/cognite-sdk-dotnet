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
    let! res = Assets.Entity.getCore 42L fetch finishEarly ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/42" @>
    test <@ ctx'.Request.Query.IsEmpty @>
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
    let! res = Assets.Entity.getCore 42L fetch finishEarly ctx

    // Assert
    test <@ Result.isError res @>
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
    let! res = Assets.Entity.getCore 42L fetch finishEarly ctx

    // Assert
    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/42" @>
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
    let! result = Assets.Entity.getCore 42L fetch finishEarly ctx

    // Assert
    test <@ Result.isOk result @>
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
    let! res = (fetch, finishEarly, ctx) |||>  Assets.Items.listCore query []

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
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
    let! res = Assets.Create.createCore fetch [] finishEarly ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets" @>
    test <@ ctx'.Request.Query.IsEmpty @>
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
    let! res = Assets.Create.createCore fetch [ asset ] finishEarly ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets" @>
    test <@ ctx'.Request.Query.IsEmpty @>

    let assets = ctx'.Response
    test <@ Seq.length assets = 1 @>
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
    let! res = Assets.Update.updateCore [ Identity.Id 42L, [] ] fetch finishEarly ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/update" @>
    test <@ ctx'.Request.Query.IsEmpty @>
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
    let! res = (fetch, finishEarly, ctx) |||> Assets.Update.updateCore  [ (Identity.Id 42L, [
        AssetUpdate.SetName "New name"
        AssetUpdate.SetDescription (Some "New description")
        AssetUpdate.ClearSource
    ])]

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/update" @>
    test <@ ctx'.Request.Query.IsEmpty @>
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

    let! res = (fetch, finishEarly, ctx) |||> Assets.Search.searchCore 100 options filter

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/search" @>
    test <@ ctx'.Request.Query.IsEmpty @>
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

    let! res = (fetch, finishEarly, ctx) |||> Assets.Items.listCore query filter

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/assets/list" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}
