// Learn more about F# at http://fsharp.org

open System
open System.Net.Http

open FsConfig

open Fusion
open Fusion.Assets
open Fusion.Timeseries
open Com.Cognite.V1.Timeseries.Proto

type Config = {
    [<CustomName("API_KEY")>]
    ApiKey: string
    [<CustomName("PROJECT")>]
    Project: string
}

let getDatapointsExample (ctx : HttpContext) = async {
    let! rsp =
        getDataPointsMultipleAsync [
            {
                Id = Identity.Id 20713436708L
                QueryOptions = [
                    GetDataPoints.QueryOption.Start "1524851819000"
                    GetDataPoints.QueryOption.End "1524859650000"
                ]
            }
        ] [] ctx

    match rsp.Result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let getAssetsExample (ctx : HttpContext) = async {
    let! rsp = getAssetsAsync [ GetAssets.Option.Limit 2 ] ctx

    match rsp.Result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let updateAssetsExample (ctx : HttpContext) = async {
    let! rsp = updateAssetsAsync [(Identity.Id 84025677715833721L, [ UpdateAssets.Option.SetName "string3" ] )]  ctx
    match rsp.Result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let searchAssetsExample (ctx : HttpContext) = async {

    let! rsp = searchAssetsAsync 10 [SearchAssets.Option.Name "VAL"] [] ctx
    match rsp.Result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}
let createAssetsExample ctx = async {

    let assets = [{
        Name = "My new asset"
        Description = Some "My description"
        MetaData = Map.empty
        Source = None
        ParentId = None
        ExternalId = None
        ParentExternalId = None
    }]

    let request = fusion {
        let! ga = createAssets assets

        let! gb = concurrent [
            createAssets assets |> retry 500<ms> 5
        ]

        return gb
    }

    let request = concurrent [
        let chunks = Seq.chunkBySize 10 assets
        for chunk in chunks do
            yield createAssets chunk |> retry 500<ms> 5
    ]

    let! result = runHandler request ctx
    match result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let insertDataPointsOldWay ctx = async {
    
    let dataPoints : NumericDataPointDto seq = seq {
        for i in 0L..100L do
            yield {
                TimeStamp = DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - i * 10L
                Value = (float ((1L + i) % 5L))
            }
        }
    let dpInsertion : InsertDataPoints.DataPoints = {
        DataPoints = Numeric dataPoints
        Identity = Identity.ExternalId "testts"
    }

    let! result = insertDataPointsAsync [ dpInsertion ] ctx
    match result.Result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let insertDataPointsProtoStyle ctx = async {
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

    let! result = insertDataPointsAsyncProto request ctx
    match result.Result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err

}

[<EntryPoint>]
let main argv =
    printfn "F# Client"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Failed to read config"

    use client = new HttpClient ()
    let ctx =
        defaultContext
        |> setHttpClient client
        |> addHeader ("api-key", Uri.EscapeDataString config.ApiKey)
        |> setProject (Uri.EscapeDataString config.Project)

    async {
        do! insertDataPointsProtoStyle ctx
    } |> Async.RunSynchronously

    0 // return an integer exit code
