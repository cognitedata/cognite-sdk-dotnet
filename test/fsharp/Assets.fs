module Tests.Assets

open System.IO

open Xunit
open Swensen.Unquote

open Cognite.Sdk
open Cognite.Sdk.Assets


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
    test <@ response.Request.Method = RequestMethod.GET @>
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
    test <@ res.Request.Method = RequestMethod.GET @>
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
    test <@ res.Request.Method = RequestMethod.POST @>
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

    let asset: AssetCreateDto = {
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
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/assets" @>
    test <@ res.Request.Query.IsEmpty @>

    match res.Result with
    | Ok assets ->
        test <@ Seq.length assets = 1 @>
    | Error error ->
        raise (Error.error2Exception error)
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
    let! res = UpdateAssets.updateAssets [ 42L, [] ] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = RequestMethod.POST @>
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
    let! res = (fetch, Async.single, ctx) |||> UpdateAssets.updateAssets  [ (42L, [
        UpdateAssets.Option.SetName "New name"
        UpdateAssets.Option.SetDescription (Some "New description")
        UpdateAssets.Option.ClearSource
    ])]

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/assets/update" @>
    test <@ res.Request.Query.IsEmpty @>
}