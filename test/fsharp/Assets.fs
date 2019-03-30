module Tests.Assets

open System.IO

open Xunit
open Swensen.Unquote
open Newtonsoft.Json

open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Request


[<Fact>]
let ``Get asset is Ok``() = async {
    // Arrange
    let json = File.ReadAllText ("Assets.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    // Act
    let! result = getAsset ctx 42L

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = GET @>
    test <@ fetcher.Ctx.Value.Resource = "/assets/42" @>
    test <@ fetcher.Ctx.Value.Query.IsEmpty @>
}

[<Fact>]
let ``Get invalid asset is Error`` () = async {
    // Arrenge
    let json = File.ReadAllText("InvalidAsset.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch


    // Act
    let! result = getAsset ctx 42L

    // Assert
    test <@ Result.isError result @>
}

[<Fact>]
let ``Get asset with extra fields is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("AssetExtra.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    // Act
    let! result = getAsset ctx 42L

    // Assert
    test <@ Result.isOk result @>
}

[<Fact>]
let ``Get asset with missing optional fields is Ok`` () = async {
    // Arrenge

    let fetcher = Fetcher.FromJson (File.ReadAllText("AssetOptional.json"))

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    // Act
    let! result = getAsset ctx 42L

    // Assert
    test <@ Result.isOk result @>
}

[<Fact>]
let ``Get assets is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    // Act
    let! result = getAssets ctx [
        Name "string"
        Path "path"
        Description "mydescription"
        MetaData (Map.ofSeq [ ("key", "value") ])
        Depth 3
        Fuzziness 2
        AutoPaging true
        NotLimit 10
        Cursor "mycursor"
    ]

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = GET @>
    test <@ fetcher.Ctx.Value.Resource = "/assets" @>

    let meta =  JsonConvert.SerializeObject(Map.ofSeq [("key", "value")]);
    test <@ fetcher.Ctx.Value.Query = [
        ("name", "string")
        ("path", "path")
        ("desc", "mydescription")
        ("metadata", meta)
        ("depth", "3")
        ("fuzziness", "2")
        ("autopaging", "true")
        ("limit", "10")
        ("cursor", "mycursor")
        ]@>
}

[<Fact>]
let ``Create assets empty is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetcher = Fetcher.FromJson json
    let fetch = fetcher.Fetch

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetch

    // Act
    let! result = createAssets ctx []

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = POST @>
    test <@ fetcher.Ctx.Value.Resource = "/assets" @>
    test <@ fetcher.Ctx.Value.Query.IsEmpty @>
}

[<Fact>]
let ``Create single asset is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    let asset: AssetCreateDto = {
        Name = "myAsset"
        Description = "Some description"
        MetaData = Map.empty
        Source = None
        SourceId = None
        RefId = None
        ParentRef = None
    }

    // Act
    let! result = createAssets ctx [ asset ]

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = POST @>
    test <@ fetcher.Ctx.Value.Resource = "/assets" @>
    test <@ fetcher.Ctx.Value.Query.IsEmpty @>

    match result with
    | Ok assets ->
        test <@ assets.Length = 1 @>
    | Error error ->
        raise (Error.error2Exception error)
}

[<Fact>]
let ``Update single asset with no updates is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    // Act
    let! result = updateAsset ctx 42L [  ]

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = POST @>
    test <@ fetcher.Ctx.Value.Resource = (sprintf "/assets/%d/update" 42L) @>
    test <@ fetcher.Ctx.Value.Query.IsEmpty @>
}

[<Fact>]
let ``Update single asset with is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    // Act
    let! result = updateAsset ctx 42L [
        SetName "New name"
        SetDescription (Some "New description")
        SetSource None
    ]

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = POST @>
    test <@ fetcher.Ctx.Value.Resource = (sprintf "/assets/%d/update" 42L) @>
    test <@ fetcher.Ctx.Value.Query.IsEmpty @>
}