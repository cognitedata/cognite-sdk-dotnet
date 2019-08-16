namespace Fusion.Events

open System.IO
open System.Net.Http

open Thoth.Json.Net

open Fusion
open Fusion.Common

[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/events/delete"

    type EventsDeleteRequest = {
        Items: Identity seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", this.Items |> Seq.map (fun item -> item.Encoder) |> Encode.seq
            ]

    let deleteCore (events: Identity seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let request : EventsDeleteRequest = {
            Items = events
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> dispose

    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="events">The list of events to delete.</param>
    /// <param name="next">Async handler to use</param>
    let delete (events: Identity seq) (next: NextHandler<unit,'a>) =
        deleteCore (events) fetch next

    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="events">The list of events to delete.</param>
    let deleteAsync<'a> (events: Identity seq) : HttpContext -> Async<Context<unit>> =
        deleteCore (events) fetch Async.single

namespace Fusion

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Fusion
open Fusion.Common

[<Extension>]
type DeleteEventExtensions =
    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="events">The list of events to delete.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtensions.Events, ids: Identity seq, [<Optional>] token: CancellationToken) : Task =
        async {
            let! ctx = Events.Delete.deleteAsync (ids) this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token) :> Task

