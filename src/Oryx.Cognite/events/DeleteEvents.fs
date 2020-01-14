// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Thoth.Json.Net
open Oryx

open CogniteSdk
open CogniteSdk.Events

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

    let deleteCore (events: Identity seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : EventsDeleteRequest = {
            Items = events
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="events">The list of events to delete.</param>
    /// <param name="next">Async handler to use</param>
    let delete (events: Identity seq) (next: NextFunc<HttpResponseMessage,'a>) =
        deleteCore events fetch next

    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="events">The list of events to delete.</param>
    let deleteAsync<'a> (events: Identity seq) : HttpContext -> HttpFuncResult<HttpResponseMessage> =
        deleteCore events fetch finishEarly

[<Extension>]
type DeleteEventExtensions =
    /// <summary>
    /// Delete multiple events in the same project
    /// </summary>
    /// <param name="ids">The list of events to delete</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: Identity seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Delete.deleteAsync (ids) ctx
            match result with
            | Ok _ -> return ()
            | Error error -> return raiseError error
        } :> Task

