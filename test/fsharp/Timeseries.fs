module Tests.Timeseries

open System
open System.IO

open Expecto
open Cognite.Sdk
open Cognite.Sdk.Timeseries
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
let timeseriesTests = testList "Timeseries tests" [
    testAsync "Create timeseries is Ok" {
        // Arrenge
        let response = File.ReadAllText("../json/Assets.json")
        let fetcher = Ok response |> Fetcher
        let fetch = fetcher.Fetch

        let ctx =
            defaultContext
            |> addHeader ("api-key", "test-key")
            |> setFetch fetch

        // Act
        let! result = createTimeseries ctx []

        // Assert
        Expect.isOk result "Should be OK"
        Expect.isSome fetcher.Ctx "Should be set"
        Expect.equal fetcher.Ctx.Value.Method POST "Should be equal"
        Expect.equal fetcher.Ctx.Value.Resource "/timeseries" "Should be equal"
        Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
    }
]