// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net

open CogniteSdk

[<AutoOpen>]
module Retrieve =
    [<Literal>]
    let Url = "/events/byids"
    type EventRequest = {
        Items: Identity seq
    } with
        member this.Encoder  =
            Encode.object [
                yield "items", this.Items |> Seq.map(fun id -> id.Encoder) |> Encode.seq
            ]

    type EventResponse = {
        Items: EventReadDto seq
    } with
         static member Decoder : Decoder<EventResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list EventReadDto.Decoder |> Decode.map seq)
            })

    let getByIdsCore (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse EventResponse.Decoder (fun response -> response.Items)
        let request : EventRequest = { Items = ids }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Events with given ids.</returns>
    let getByIds (ids: Identity seq) (next: NextFunc<EventReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        getByIdsCore ids fetch next

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <returns>Events with given ids.</returns>
    let getByIdsAsync (ids: Identity seq) =
        getByIdsCore ids fetch Async.single


[<Extension>]
type GetEventsByIdsClientExtensions =
    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <returns>Events with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<Identity>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        async {
            let! ctx = Retrieve.getByIdsAsync ids this.Ctx
            match ctx.Result with
            | Ok events ->
                return events |> Seq.map (fun event -> event.ToEventEntity ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <returns>Events with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<int64>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map (fun x -> Identity.Id x), token)

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <returns>Events with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<string>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map (fun x -> Identity.ExternalId x), token)



