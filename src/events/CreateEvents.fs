// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.ResponseReaders
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.Events


[<RequireQualifiedAccess>]
module Create =
    [<Literal>]
    let Url = "/events"

    type Request = {
        Items: EventWriteDto seq
    } with
         member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (it: EventWriteDto) -> it.Encoder) this.Items |> Encode.seq
            ]

    type EventResponse = {
        Items: EventReadDto seq
    } with
         static member Decoder : Decoder<EventResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list EventReadDto.Decoder |> Decode.map seq)
            })

    let createCore (events: EventWriteDto seq) (fetch: HttpHandler<HttpResponseMessage,'a>)  =
        let request : Request = { Items = events }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json EventResponse.Decoder
        >=> map (fun res -> res.Items)

    /// <summary>
    /// Create one or more events.
    /// </summary>
    /// <param name="events">List of events to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created events.</returns>
    let create (events: EventWriteDto seq) (next: NextFunc<EventReadDto seq, 'a>) =
        createCore events fetch next

    /// <summary>
    /// Create new events in the given project.
    /// </summary>
    /// <param name="events">The events to create.</param>
    /// <returns>List of created events.</returns>
    let createAsync (events: EventWriteDto seq) =
        createCore events fetch finishEarly

[<Extension>]
type CreateEventExtensions =
    /// <summary>
    /// Create new events in the given project.
    /// </summary>
    /// <param name="events">The events to create.</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>List of created events.</returns>
    [<Extension>]
    static member CreateAsync(this: ClientExtension, events: EventEntity seq, [<Optional>] token: CancellationToken) : Task<EventEntity seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let events' = events |> Seq.map EventWriteDto.FromEventEntity
            let! result = Create.createAsync events' ctx
            match result with
            | Ok ctx ->
                return ctx.Response |> Seq.map (fun event -> event.ToEventEntity ())
            | Error error -> return raiseError error
        }
