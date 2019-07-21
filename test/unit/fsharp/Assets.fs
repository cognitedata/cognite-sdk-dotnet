module Tests.Assets

open System.IO

open Xunit
open Swensen.Unquote

open Fusion
open Fusion.Assets
open System.Net.Http


[<Fact>]
let ``Get asset is Ok``() = async {
    // Arrange
    let json = File.ReadAllText "Asset.json"
    let fetch = Fetch.fromJson json
    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! response = GetAsset.getAsset 42L fetch Async.single ctx

    // Assert
    test <@ Result.isOk response.Result @>
    test <@ response.Request.Method = HttpMethod.Get @>
    test <@ response.Request.Resource = "/assets/42" @>
    test <@ response.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get invalid asset is Error`` () = async {
    // Arrenge
    let json = File.ReadAllText "InvalidAsset.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")


    // Act
    let! response = GetAsset.getAsset 42L fetch Async.single ctx

    // Assert
    test <@ Result.isError response.Result @>
}

[<Fact>]
let ``Get asset with extra fields is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "AssetExtra.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! response = GetAsset.getAsset 42L fetch Async.single ctx

    // Assert
    test <@ Result.isOk response.Result @>
}

[<Fact>]
let ``Get asset with missing optional fields is Ok`` () = async {
    // Arrenge

    let json = File.ReadAllText "AssetOptional.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! response = GetAsset.getAsset 42L fetch Async.single ctx

    // Assert
    test <@ Result.isOk response.Result @>
}

[<Fact>]
let ``Get assets is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
    let args = [
            GetAssets.Option.Name "string"
            GetAssets.Option.Source "source"
            GetAssets.Option.Root false
            GetAssets.Option.ParentIds [42L; 43L]
            GetAssets.Option.Limit 10
            GetAssets.Option.Cursor "mycursor"
        ]

    // Act
    let! res = (fetch, Async.single, ctx) |||>  GetAssets.getAssets args

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Get @>
    test <@ res.Request.Resource = "/assets" @>

    test <@ res.Request.Query = [
        ("name", "string")
        ("source", "source")
        ("root", "false")
        ("parentIds", "[42,43]")
        ("limit", "10")
        ("cursor", "mycursor")
    ] @>
}

[<Fact>]
let ``Create assets empty is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! res = CreateAssets.createAssets [] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/assets" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Create single asset is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

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
    let! res = CreateAssets.createAssets [ asset ] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/assets" @>
    test <@ res.Request.Query.IsEmpty @>

    match res.Result with
    | Ok assets ->
        test <@ Seq.length assets = 1 @>
    | Error error ->
        raise (Sdk.XunitException (error.ToString ()))
}

[<Fact>]
let ``Update single asset with no updates is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! res = UpdateAssets.updateAssets [ Identity.Id 42L, [] ] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/assets/update" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Update single asset with is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! res = (fetch, Async.single, ctx) |||> UpdateAssets.updateAssets  [ (Identity.Id 42L, [
        UpdateAssets.Option.SetName "New name"
        UpdateAssets.Option.SetDescription (Some "New description")
        UpdateAssets.Option.ClearSource
    ])]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/assets/update" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Attempt searching assets`` () = async {
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
    let options = [
        SearchAssets.Option.Name "str"
    ]
    let filter = [
        AssetFilter.Name "string"
    ]

    let! res = (fetch, Async.single, ctx) |||> SearchAssets.searchAssets 100 options filter

    test <@ Result.isOk res. Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/assets/search" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Attempt filtering assets`` () = async {
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
    let options = [
        FilterAssets.Option.Limit 100
    ]
    let filter = [
        AssetFilter.Name "string"
    ]

    let! res = (fetch, Async.single, ctx) |||> FilterAssets.filterAssets options filter

    test <@ Result.isOk res. Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/assets/list" @>
    test <@ res.Request.Query.IsEmpty @>
}
