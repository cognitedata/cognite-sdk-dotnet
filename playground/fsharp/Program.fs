// Learn more about F# at http://fsharp.org

open System
open System.Net.Http

open FsConfig
open Com.Cognite.V1.Timeseries.Proto

open Oryx
open Oryx.Retry

open CogniteSdk
open CogniteSdk.Assets
open CogniteSdk.TimeSeries
open CogniteSdk.DataPoints
open FSharp.Control.Tasks.V2.ContextInsensitive

type Config = {
    [<CustomName("API_KEY")>]
    ApiKey: string
    [<CustomName("PROJECT")>]
    Project: string
}

let getDatapointsExample (ctx : HttpContext) = task {
    let! res =
        DataPoints.Items.listMultipleAsync [
            {
                Id = Identity.Id 20713436708L
                QueryOptions = [
                    DataPointQuery.Start "1524851819000"
                    DataPointQuery.End "1524859650000"
                ]
            }
        ] [] ctx

    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let getAssetsExample (ctx : HttpContext) = task {
    let! res = Assets.Items.listAsync [ AssetQuery.Limit 2 ] [] ctx

    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let updateAssetsExample (ctx : HttpContext) = task {
    let! res = Assets.Update.updateAsync [ (Identity.Id 84025677715833721L, [ AssetUpdate.SetName "string3" ] )]  ctx
    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let searchAssetsExample (ctx : HttpContext) = task {

    let! res = Assets.Search.searchAsync 10 [ AssetSearch.Name "VAL" ] [] ctx
    match res with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}
let createAssetsExample ctx = task {

    let assets = [{
        Name = "My new asset"
        Description = Some "My description"
        MetaData = Map.empty
        Source = None
        ParentId = None
        ExternalId = None
        ParentExternalId = None
    }]

    let request = oryx {
        let! ga = Assets.Create.create assets

        let! gb = concurrent [
            retry 500<ms> 5 >=> Assets.Create.create assets
        ]

        return gb
    }

    let request = concurrent [
        let chunks = Seq.chunkBySize 10 assets
        for chunk in chunks do
            yield retry 500<ms> 5 >=> Assets.Create.create chunk
    ]

    let! result = runAsync request ctx
    match result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let insertDataPointsOldWay ctx = task {

    let dataPoints : NumericDataPointDto seq = seq {
        for i in 0L..100L do
            yield {
                TimeStamp = DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - i * 10L
                Value = (float ((1L + i) % 5L))
            }
        }
    let dpInsertion : DataPoints.Insert.DataPoints = {
        DataPoints = Numeric dataPoints
        Identity = Identity.ExternalId "testts"
    }

    let! result = DataPoints.Insert.insertAsync [ dpInsertion ] ctx
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

    let! result = DataPoints.Insert.insertAsyncProto request ctx
    match result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err

}

let asyncMain argv =task {
    printfn "F# Client"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Failed to read config"

    use client = new HttpClient ()
    let ctx =
        Context.create ()
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
