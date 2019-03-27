module Tests.Timeseries

open System
open System.IO

open Xunit
open Swensen.Unquote

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

[<Fact>]
let ``Create timeseries is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText("Assets.json")
    let fetcher = Fetcher.FromJson json
    let fetch = fetcher.Fetch

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")
        |> setFetch fetch

    // Act
    let! result = createTimeseries ctx []

    // Assert
    test <@ Result.isOk result @>
    test <@ Option.isSome fetcher.Ctx @>
    test <@ fetcher.Ctx.Value.Method = POST @>
    test <@ fetcher.Ctx.Value.Resource = "/timeseries" @>
    test <@ fetcher.Ctx.Value.Query.IsEmpty @>
}
