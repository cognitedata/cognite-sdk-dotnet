module Tests.Test

open System
open System.IO

open Expecto
open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Context


type Fetcher (response) =
    let mutable _ctx: Context option = None

    member this.Ctx =
        _ctx

    member this.Fetch (ctx: Context) = async {
        _ctx <- Some ctx
        return response
    }

[<Tests>]
let assetTests = testList "A test group" [
    testAsync "Get asset is Ok" {
        // Arrenge
        let response = File.ReadAllText("Assets.json")
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
        Expect.equal fetcher.Ctx.Value.Method Get "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource (Resource "/assets/42") "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
    }

    testAsync "Get assets is Ok" {
        // Arrenge
        let response = File.ReadAllText("Assets.json")
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
        Expect.equal fetcher.Ctx.Value.Method Get "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource (Resource "/assets") "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [("name", "string")] "Should be equal"
    }

    testAsync "Create asset empty is Ok" {
        // Arrenge
        let response = File.ReadAllText("Assets.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = createAsset ctx []

        // Assert
        Expect.isOk result "Should be OK"
        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method Post "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource (Resource "/assets") "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
    }

    testAsync "Create asset is Ok" {
        // Arrenge
        let response = File.ReadAllText("Assets.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        let asset: Asset = {

        }

        // Act
        let! result = createAsset ctx []

        // Assert
        Expect.isOk result "Should be OK"
        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method Post "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource (Resource "/assets") "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
    }
]