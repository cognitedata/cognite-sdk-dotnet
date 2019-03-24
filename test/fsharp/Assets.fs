module Tests.Assets

open System.IO

open Expecto

open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Request


[<Tests>]
let assetTests = testList "Asset tests" [
    testAsync "Get asset is Ok" {
        // Arrenge
        let json = File.ReadAllText ("../json/Assets.json")
        let fetcher = Fetcher.FromJson json

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetcher.Fetch

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
        let json = File.ReadAllText("../json/InvalidAsset.json")
        let fetcher = Fetcher.FromJson json

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetcher.Fetch


        // Act
        let! result = getAsset ctx 42L

        // Assert
        Expect.isError result "Should be Error"
    }

    testAsync "Get asset with extra fields is Ok" {
        // Arrenge
        let json = File.ReadAllText("../json/AssetExtra.json")
        let fetcher = Fetcher.FromJson json

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetcher.Fetch

        // Act
        let! result = getAsset ctx 42L

        // Assert
        Expect.isOk result "Should be Ok"
    }

    testAsync "Get asset with missing optional fields is Ok" {
        // Arrenge

        let fetcher = Fetcher.FromJson (File.ReadAllText("../json/AssetOptional.json"))

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetcher.Fetch

        // Act
        let! result = getAsset ctx 42L

        // Assert
        Expect.isOk result "Should be Ok"
    }

    testAsync "Get assets is Ok" {
        // Arrenge
        let json = File.ReadAllText("../json/Assets.json")
        let fetcher = Fetcher.FromJson json

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetcher.Fetch

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
        let json = File.ReadAllText("../json/Assets.json")
        let fetcher = Fetcher.FromJson json
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
        let json = File.ReadAllText("../json/Assets.json")
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
        Expect.isOk result "Should be OK"
        match result with
        | Ok assets ->
            Expect.equal assets.Length 1 "Should be equal"
        | Error error ->
            raise (Error.error2Exception error)

        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method POST "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource "/assets" "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
    }
]