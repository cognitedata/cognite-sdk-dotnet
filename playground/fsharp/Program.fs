// Learn more about F# at http://fsharp.org

open System
open FsConfig

open Cognite.Sdk
open Cognite.Sdk.Request
open Cognite.Sdk.Assets
//open Cognite.Sdk.Builder

type Config = {
    [<CustomName("API_KEY")>]
    ApiKey: string
    [<CustomName("PROJECT")>]
    Project: string
}

let getAssetsExample ctx = async {
    let! result = getAssets [ Limit 2 ] ctx

    match result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let updateAssetExample (ctx : HttpContext) = async {

    let! result = updateAsset 84025677715833721L [ SetName "string3" ] ctx

    match result with
    | Ok res -> printfn "%A" res
    | Error err -> printfn "Error: %A" err
}

let createAssetsExample ctx = async {

    let assets = [{
        Name = "My new asset"
        Description = "My description"
        MetaData = Map.empty
        Source = None
        SourceId = None
        RefId = None
        ParentRef = None
    }]

    let request = req {
        let! ga = createAssets assets

        let! gb = Handler.concurrent [
            createAssets assets
            |> Handler.retry 500<ms> 5
        ]

        return ga
    }


    let request = Handler.concurrent [
        createAssets assets.[0..9] |> Handler.retry 500<ms> 5
        createAssets assets.[10..19] |> Handler.retry 500<ms> 5
        createAssets assets.[20..29] |> Handler.retry 500<ms> 5
    ]

    let! result = request ctx

    //match result with
    //| Ok res -> printfn "%A" res
    //| Error err -> printfn "Error: %A" err
    ()
}

[<EntryPoint>]
let main argv =
    printfn "F# Client"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Failed to read config"

    let ctx =
        defaultContext
        |> addHeader ("api-key", Uri.EscapeDataString config.ApiKey)
        |> setProject (Uri.EscapeDataString config.Project)

    async {
        do! getAssetsExample ctx
    } |> Async.RunSynchronously

    0 // return an integer exit code



