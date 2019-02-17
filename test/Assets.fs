module Tests.Test

open System
open System.IO

open Expecto
open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Context


let fetcher result (ctx: Context) =
    async {
        return result
    }

[<Tests>]
let assetTests =
    testAsync "Get asset is Ok" {
        // Arrenge
        let response = File.ReadAllText("Assets.json")
        let fetch = Ok response |> fetcher

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test")
            |> setFetch fetch

        // Act
        let! result = getAssets ctx [ Name "string"]

        // Assert
        Expect.isOk result "Should be OK"
    }