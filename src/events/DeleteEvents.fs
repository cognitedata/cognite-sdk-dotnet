// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

open System.IO
open System.Net.Http

open Thoth.Json.Net
open Oryx
open CogniteSdk

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
    let delete (events: Identity seq) (next: NextFunc<unit,'a>) =
        deleteCore events fetch next

    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="events">The list of events to delete.</param>
    let deleteAsync<'a> (events: Identity seq) : HttpContext -> Async<Context<unit>> =
        deleteCore events fetch Async.single

namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open CogniteSdk
open CogniteSdk.Events

[<Extension>]
type DeleteEventExtensions =
    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="ids">The list of events to delete</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: Identity seq, [<Optional>] token: CancellationToken) : Task =
        async {
            let! ctx = Events.Delete.deleteAsync (ids) this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token) :> Task

