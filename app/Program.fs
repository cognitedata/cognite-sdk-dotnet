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
        let! result = getAssets ctx [ Limit 2]

        //let! result = updateAsset ctx 8402567771583372L [ SetName "string3" ]
        match result with
        | Ok res -> printfn "%A" res
        | Error err -> printfn "Error: %A" err

    } |> Async.RunSynchronously

    0 // return an integer exit code
