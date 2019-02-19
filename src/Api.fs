namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks;

open Cognite.Sdk
open Cognite.Sdk.Context
open Cognite.Sdk.Assets


type AssetArgs (args: Args list) =
    let args  = args

    member this.Name(name: string) =
        let arg = Name name
        let newArgs = arg :: args
        AssetArgs(newArgs)

    member internal this.Args = args

    static member Create() =
        AssetArgs([])


type Client (context: Context) =
    let context = context

    new() = Client(defaultContext)

    member internal this.Ctx =
        context

    member this.AddHeader(name: string, value: string)  =
        context
        |> addHeader (name, value)
        |> Client

    member this.SetProject(project: string) =
        context
        |> setProject(project)
        |> Client

    static member Create() =
        Client(defaultContext)

    member this.GetAssets (args: AssetArgs) : Task<AssetResponse> =
        let worker () : Async<AssetResponse> = async {
            let! result = getAssets context args.Args
            match result with
            | Ok response ->
                return response
            | Error ex -> 
                return raise ex
        }

        let task = worker () |> Async.StartAsTask
        task
