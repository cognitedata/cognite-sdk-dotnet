namespace Fusion

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Api
open Fusion.Common

[<RequireQualifiedAccess>]
module DeleteTimeseries =
    [<Literal>]
    let Url = "/timeseries/delete"

    type DeleteRequest = {
        Items: seq<Identity>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: Identity) -> it.Encoder) this.Items |> Encode.seq)
            ]

    let deleteTimeseries (items: Identity seq) (fetch: HttpHandler<HttpResponseMessage, Stream, unit>) =
        let request : DeleteRequest = { Items = items }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> dispose

[<AutoOpen>]
module DeleteTimeseriesApi =
    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    /// <param name="next">Async handler to use.</param>
    let deleteTimeseries (items: Identity seq) (next: NextHandler<unit, unit>) =
        DeleteTimeseries.deleteTimeseries items fetch next

    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    let deleteTimeseriesAsync (items: Identity seq) =
        DeleteTimeseries.deleteTimeseries items fetch Async.single

[<Extension>]
type DeleteTimeseriesExtensions =
    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    [<Extension>]
    static member DeleteTimeseriesAsync (this: Client) (items: Identity seq) : Task =
        task {
            let! ctx = deleteTimeseriesAsync items this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } :> Task
