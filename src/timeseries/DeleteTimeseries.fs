namespace CogniteSdk.TimeSeries

open System.IO
open System.Net.Http

open Oryx
open Thoth.Json.Net

open CogniteSdk


[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/timeseries/delete"

    type DeleteRequest = {
        Items: seq<Identity>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: Identity) -> it.Encoder) this.Items |> Encode.seq)
            ]

    let deleteCore (items: Identity seq) (fetch: HttpHandler<HttpResponseMessage, Stream, unit>) =
        let request : DeleteRequest = { Items = items }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> dispose

    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    /// <param name="next">Async handler to use.</param>
    let delete (items: Identity seq) (next: NextHandler<unit, unit>) =
        deleteCore items fetch next

    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    let deleteAsync (items: Identity seq) =
        deleteCore items fetch Async.single

namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Oryx
open CogniteSdk.TimeSeries

[<Extension>]
type DeleteTimeSeriesClientExtensions =
    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    [<Extension>]
    static member DeleteAsync (this: TimeSeriesClientExtension, items: Identity seq, [<Optional>] token: CancellationToken) : Task =
        async {
            let! ctx = Delete.deleteAsync items this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token) :> Task
