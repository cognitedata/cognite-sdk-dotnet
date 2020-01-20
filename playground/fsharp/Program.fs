// Learn more about F# at http://fsharp.org

open System
open System.Net.Http

open FsConfig
open Com.Cognite.V1.Timeseries.Proto 

open Oryx
open Oryx.Retry
open Oryx.Cognite

open CogniteSdk
open FSharp.Control.Tasks.V2.ContextInsensitive
open System.Threading

type Config = {
    [<CustomName("API_KEY")>]
    ApiKey: string
    [<CustomName("PROJECT")>]
    Project: string
}

let getDatapointsExample (ctx : HttpContext) = task {
    let query = 
        DataPoints.DataPointsQuery(
            Items = [ DataPoints.DataPointsQueryItem(Id=20713436708L) ],
            Start = Nullable 1524851819000L,
            End = Nullable 1524859650000L
        )
    let! res = 
        DataPoints.list query
        |> runUnsafeAsync ctx CancellationToken.None
    printfn "%A" res
 }

let getAssetsExample (ctx : HttpContext) = task {
    printfn "FIRST **********************************"
    let! res = 
        Assets.AssetQueryDto(Limit = Nullable 2)
        |> Assets.list
        |> runUnsafeAsync ctx CancellationToken.None
    
    printfn "SECOND **********************************"
    let! res = 
        Assets.AssetQueryDto(Limit = Nullable 2) 
        |> Assets.list
        |> runAsync ctx
    match res with
    | Ok res -> () //printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let updateAssetsExample (ctx : HttpContext) = task {
    let query =  [
        UpdateItem(
            Id = 84025677715833721L,
            Update = Assets.AssetUpdateDto(Name = SetUpdate(Set="string3"))
        )
    ]
    let! res = Assets.update query |> runAsync ctx
    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let searchAssetsExample (ctx : HttpContext) = task {

    let query = 
        SearchQueryDto<Assets.AssetFilterDto, SearchDto>(
            Search = SearchDto(Name = "VAL"),
            Limit = Nullable 10
        )
    let! res = Assets.search query |> runAsync ctx
    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}
let createAssetsExample ctx = task {

    let assets = [
        Assets.AssetWriteDto(
            Name = "My new asset",
            Description = "My description"
       )
    ]

    let myRetry = retry 500<ms> 5

    let request = myRetry >=> req {
        let! ga = Assets.create assets

        let! gb = concurrent [
            retry 500<ms> 5 >=> Assets.create assets
        ]

        return gb
    }

    let request = myRetry >=> concurrent [
        let chunks = Seq.chunkBySize 10 assets
        for chunk in chunks do
            yield retry 500<ms> 5 >=> Assets.create chunk
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

let asyncMain argv = task {
    printfn "F# Client"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Failed to read config"

    use client = new HttpClient ()
    let ctx =
        Context.defaultContext
        |> Context.setAppId "playground"
        |> Context.setHttpClient client
        |> Context.addHeader ("api-key", Uri.EscapeDataString config.ApiKey)
        |> Context.setProject (Uri.EscapeDataString config.Project)

    do! getAssetsExample ctx
}

[<EntryPoint>]
let main argv =
    asyncMain().GetAwaiter().GetResult()

    0 // return an integer exit code
