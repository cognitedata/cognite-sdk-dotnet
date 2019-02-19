namespace Cognite.Sdk.Api

open System.Collections.Generic
open System.Threading.Tasks;

open Cognite.Sdk.Context
open Cognite.Sdk.Assets

type Context (context) =
    let context = context

    new() = Context(defaultContext)

    member internal this.Ctx =
        context

    member this.AddHeader(name: string, value: string)  =
        context
        |> addHeader (name, value)
        |> Context

    member this.SetProject(project: string) =
        context
        |> setProject(project)
        |> Context

    static member Create() =
        Context(defaultContext)


type Args () =
    static member Name() =
        Cognite.Sdk.Assets.Args.Name

type Client () =
    member this.GetAssets(ctx: Context, args: int List) : Task<AssetResponse> =
        let worker () : Async<AssetResponse> = async {
            let! result = getAssets ctx.Ctx [] 
            match result with
            | Ok response ->
                return response
            | Error ex -> 
                return raise ex
        }

        let task = worker () |> Async.StartAsTask
        task
