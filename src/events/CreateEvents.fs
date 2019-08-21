// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

open System.IO
open System.Net.Http

open Thoth.Json.Net
open CogniteSdk
open Oryx



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

    let createCore (events: EventWriteDto seq) (fetch: HttpHandler<HttpResponseMessage,Stream,'a>)  =
        let decoder = Encode.decodeResponse EventResponse.Decoder (fun res -> res.Items)
        let request : Request = { Items = events }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

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
        createCore events fetch Async.single

namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices

open CogniteSdk.Events
open System.Threading

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
        async {
            let events' = events |> Seq.map EventWriteDto.FromEventEntity
            let! ctx = Create.createAsync events' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun event -> event.ToEventEntity ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
