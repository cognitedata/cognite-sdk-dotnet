// Learn more about F# at http://fsharp.org

open System
open System.Net.Http

open FsConfig
open Com.Cognite.V1.Timeseries.Proto

open Oryx
open Oryx.Cognite

open CogniteSdk
open FSharp.Control.Tasks

type Config = {
    [<CustomName("API_KEY")>]
    ApiKey: string
    [<CustomName("PROJECT")>]
    Project: string
}

let getDatapointsExample (ctx : HttpContext) = task {
    let query =
        DataPointsQuery(
            Items = [ DataPointsQueryItem(Id=Nullable 20713436708L) ],
            Start = "1524851819000",
            End = "1524859650000"
        )
    let! res =
        DataPoints.list query
        |> runUnsafeAsync ctx
    printfn "%A" res
 }

let getAssetsExample (ctx : HttpContext) = task {
    let! res =
        AssetQuery(Limit = Nullable 2)
        |> Assets.list
        |> runUnsafeAsync ctx

    let! res =
        AssetQuery(Limit = Nullable 2)
        |> Assets.list
        |> runAsync ctx
    match res with
    | Ok res -> () //printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let updateAssetsExample (ctx : HttpContext) = task {
    let query =  [
        UpdateItem(
            id = 84025677715833721L,
            Update = AssetUpdate(Name = Update("string3"))
        )
    ]
    let! res = Assets.update query |> runAsync ctx
    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let searchAssetsExample (ctx : HttpContext) = task {

    let query =
        AssetSearch(
            Search = Search(Name = "VAL"),
            Limit = Nullable 10
        )
    let! res = Assets.search query |> runAsync ctx
    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let createAssetsExample ctx = task {

    let assets = [
        AssetCreate(
            Name = "My new asset",
            Description = "My description"
       )
    ]

    let request = req {
        let! ga = Assets.create assets
        let! gb = Assets.create assets

        return gb
    }

    let request = concurrent [
        let chunks = Seq.chunkBySize 10 assets
        for chunk in chunks do
            yield Assets.create chunk
    ]

    let! result = request |> runAsync ctx
    match result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let insertDataPointsProtoStyle ctx = task {
    let request = DataPointInsertionRequest ()
    let dataPoints = seq {
        for i in 0L..100L do
            let point = NumericDatapoint ()
            point.Timestamp <- DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - i * 10L
            point.Value <- float ((1L + i) % 5L)
            yield point
    }
    let item = DataPointInsertionItem ()
    item.NumericDatapoints <- NumericDatapoints ()
    item.NumericDatapoints.Datapoints.AddRange(dataPoints)
    item.ExternalId <- "testts"
    request.Items.Add(item)

    let! result =
        DataPoints.create request
        |> runAsync ctx
    match result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err

}

let syntheticQueryExample (ctx : HttpContext) = task {
    let query =
        TimeSeriesSyntheticQuery(
            Items = [
                TimeSeriesSyntheticQueryItem(
                    Expression="ts{externalId='pi:160627'} + 1",
                    Start = "30d-ago"
                )
            ]
        )
    let! res =
        TimeSeries.syntheticQuery query
        |> runUnsafeAsync ctx
    printfn "%A" res
 }

let asyncMain argv = task {
    printfn "F# Client"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Failed to read config"

    use client = new HttpClient ()
    let ctx =
        HttpContext.create ()
        |> HttpContext.withAppId "playground"
        |> HttpContext.withHttpClient client
        |> HttpContext.withHeader ("api-key", Uri.EscapeDataString config.ApiKey)
        |> HttpContext.withProject (Uri.EscapeDataString config.Project)

    do! getAssetsExample ctx
}

[<EntryPoint>]
let main argv =
    asyncMain().GetAwaiter().GetResult()

    0 // return an integer exit code
