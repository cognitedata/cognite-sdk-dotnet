module Tests.Assets

open System.IO

open Xunit
open Swensen.Unquote

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

let ``Get assets is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetcher = Fetcher.FromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetcher.Fetch

    // Act
    let! result = getAssets ctx [ Name "string"]

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = GET @>
    test <@ fetcher.Ctx.Value.Resource = "/assets" @>
    test <@ fetcher.Ctx.Value.Query = [("name", "string")] @>
}

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


    (*
    match result with
    | Ok assets ->
        Expect.equal assets.Length 1 "Should be equal"
    | Error error ->
        raise (Error.error2Exception error)

    *)
}