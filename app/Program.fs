// Learn more about F# at http://fsharp.org

open System
open FsConfig

open Cognite.Sdk.Assets
open Cognite.Sdk.Context

type Config = {
    [<CustomName("API_KEY")>]
    ApiKey: string
    [<CustomName("PROJECT")>]
    Project: string
}

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"

    let config =
        match EnvConfig.Get<Config>() with
        | Ok config -> config
        | Error error -> failwith "Unable to read config"

    let ctx =
        defaultContext
        |> addHeader ("api-key", Uri.EscapeDataString config.ApiKey)
        |> setProject (Uri.EscapeDataString config.Project)
        // |> setFetch = Request.fetch

    async {
        let! result = getAssets ctx [ Name "string"]
        match result with
        | Ok res -> printfn "%A" res
        | Error err -> printfn "Error: %A" err

    } |> Async.RunSynchronously

    0 // return an integer exit code
