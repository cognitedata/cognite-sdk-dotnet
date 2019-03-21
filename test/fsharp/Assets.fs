module Tests.Test

open System
open System.IO

open Expecto
open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Request


type Fetcher (response: Result<string, ResponseError>) =
    let mutable _ctx: Context option = None

    member this.Ctx =
        _ctx

    member this.Fetch (ctx: Context) = async {
        _ctx <- Some ctx
        return response
    }

[<Tests>]
let assetTests = testList "Asset tests" [
    testAsync "Get asset is Ok" {
        // Arrenge
        let response = File.ReadAllText("../json/Assets.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = getAsset ctx 42L

        // Assert
        Expect.isOk result "Should be OK"
        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method GET "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource "/assets/42" "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
    }

    testAsync "Get invalid asset is Error" {
        // Arrenge
        let response = File.ReadAllText("../json/InvalidAsset.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = getAsset ctx 42L

        // Assert
        Expect.isError result "Should be Error"
    }

    testAsync "Get asset with extra fields is Ok" {
        // Arrenge
        let response = File.ReadAllText("../json/AssetExtra.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = getAsset ctx 42L

        // Assert
        Expect.isOk result "Should be Ok"
    }

    testAsync "Get asset with missing optional fields is Ok" {
        // Arrenge
        let response = File.ReadAllText("../json/AssetOptional.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = getAsset ctx 42L

        // Assert
        Expect.isOk result "Should be Ok"
    }

    testAsync "Get assets is Ok" {
        // Arrenge
        let response = File.ReadAllText("../json/Assets.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = getAssets ctx [ Name "string"]

        // Assert
        Expect.isOk result "Should be OK"
        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method GET "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource "/assets" "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [("name", "string")] "Should be equal"
    }

    testAsync "Create assets empty is Ok" {
        // Arrenge
        let response = File.ReadAllText("../json/Assets.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = createAssets ctx []

        // Assert
        Expect.isOk result "Should be OK"
        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method POST "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource "/assets" "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
    }

    testAsync "Create single asset is Ok" {
        // Arrenge
        let response = File.ReadAllText("../json/Assets.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        let asset: CreateAssetDto = {
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
        Expect.isOk result "Should be OK"
        match result with
        | Ok assets ->
            Expect.equal assets.Length 1 "Should be equal"
        | Error error ->
            raise (Request.error2Exception error)

        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method POST "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource "/assets" "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
        printfn "%A" fetcher.Ctx.Value.Body

    }
]