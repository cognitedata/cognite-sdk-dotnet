// Learn more about F# at http://fsharp.org

open System
open System.Net.Http

open FsConfig
open Com.Cognite.V1.Timeseries.Proto

open Microsoft.Identity.Client

open Oryx
open Oryx.Cognite

open CogniteSdk
open FSharp.Control.TaskBuilder

type Config = {
    [<CustomName("TENANT_ID")>]
    TenantId: string
    [<CustomName("CLIENT_ID")>]
    ClientId: string
    [<CustomName("CLIENT_SECRET")>]
    ClientSecret: string
    [<CustomName("CDF_CLUSTER")>]
    Cluster: string
    [<CustomName("CDF_PROJECT")>]
    Project: string
}

let getDatapointsExample (ctx: HttpHandler<unit>) =
    task {
        let query =
            DataPointsQuery(
                Items = [ DataPointsQueryItem(Id = Nullable 20713436708L) ],
                Start = "1524851819000",
                End = "1524859650000"
            )

        let! res = ctx |> DataPoints.list query |> runUnsafeAsync
        printfn "%A" res
    }

let getAssetsExample (ctx: HttpHandler<unit>) =
    task {
        let! res = ctx |> Assets.list (AssetQuery(Limit = Nullable 2)) |> runUnsafeAsync

        let! res = ctx |> Assets.list (AssetQuery(Limit = Nullable 2)) |> runAsync

        match res with
        | Ok res -> () //printfn "%A" res
        | Error err -> printfn "Error: %A" err
    }

let updateAssetsExample (ctx: HttpHandler<unit>) =
    task {
        let query =
            [ UpdateItem(id = 84025677715833721L, Update = AssetUpdate(Name = Update("string3"))) ]

        let! res = ctx |> Assets.update query |> runAsync

        match res with
        | Ok res -> printfn "%A" res
        | Error err -> printfn "Error: %A" err
    }

let searchAssetsExample (ctx: HttpHandler<unit>) =
    task {

        let query = AssetSearch(Search = Search(Name = "VAL"), Limit = Nullable 10)
        let! res = ctx |> Assets.search query |> runAsync

        match res with
        | Ok res -> printfn "%A" res
        | Error err -> printfn "Error: %A" err
    }

let createAssetsExample ctx =
    task {

        let assets = [ AssetCreate(Name = "My new asset", Description = "My description") ]

        let request =
            http {
                let! ga = ctx |> Assets.create assets
                let! gb = ctx |> Assets.create assets

                return gb
            }

        let request =
            concurrent
                [ let chunks = Seq.chunkBySize 10 assets

                  for chunk in chunks do
                      yield ctx |> Assets.create chunk ]

        let! result = request |> runAsync

        match result with
        | Ok res -> printfn "%A" res
        | Error err -> printfn "Error: %A" err
    }

let insertDataPointsProtoStyle ctx =
    task {
        let request = DataPointInsertionRequest()

        let dataPoints =
            seq {
                for i in 0L .. 100L do
                    let point = NumericDatapoint()

                    point.Timestamp <- DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - i * 10L

                    point.Value <- float ((1L + i) % 5L)
                    yield point
            }

        let item = DataPointInsertionItem()
        item.NumericDatapoints <- NumericDatapoints()
        item.NumericDatapoints.Datapoints.AddRange(dataPoints)
        item.ExternalId <- "testts"
        request.Items.Add(item)

        let! result = ctx |> DataPoints.create request |> runAsync

        match result with
        | Ok res -> printfn "%A" res
        | Error err -> printfn "Error: %A" err

    }

let syntheticQueryExample (ctx: HttpHandler<unit>) =
    task {
        let query =
            TimeSeriesSyntheticQuery(
                Items = [
                    TimeSeriesSyntheticQueryItem(
                        Expression = "ts{externalId='pi:PI-13148-A2'} + 1",
                        Start = "30d-ago"
                    )
                ]
            )

        let! res = ctx |> TimeSeries.syntheticQuery query |> runUnsafeAsync

        printfn "%A" res
    }

let asyncMain argv =
    task {
        printfn "F# Client"

        let config =
            match EnvConfig.Get<Config>() with
            | Ok config -> config
            | Error error -> failwith "Failed to read config"

        let scopes = [ $"https://{config.Cluster}.cognitedata.com/.default" ]

        let app = ConfidentialClientApplicationBuilder
                    .Create(config.ClientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, config.TenantId)
                    .WithClientSecret(config.ClientSecret)
                    .Build()

        let getTokenTask = task {
            let! result = app.AcquireTokenForClient(scopes).ExecuteAsync() |> Async.AwaitTask
            return result.AccessToken
        }
        let accessToken = getTokenTask.Result

        use client = new HttpClient()

        let ctx =
            HttpHandler.empty
            |> withAppId "playground"
            |> withHttpClient client
            |> withHeader ("Authorization", $"Bearer {accessToken}")
            |> withProject (Uri.EscapeDataString config.Project)
            |> withBaseUrl (Uri($"https://{config.Cluster}.cognitedata.com"))

        do! syntheticQueryExample ctx
    }

[<EntryPoint>]
let main argv =
    asyncMain().GetAwaiter().GetResult()

    0 // return an integer exit code
