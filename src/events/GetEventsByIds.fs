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
open FSharp.Control.Tasks.V2.ContextInsensitive

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

    let getByIdsCore (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse EventResponse.Decoder (fun response -> response.Items)
        let request : EventRequest = { Items = ids }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Events with given ids.</returns>
    let getByIds (ids: Identity seq) (next: NextFunc<EventReadDto seq,'a>) : HttpContext -> Task<Context<'a>> =
        getByIdsCore ids fetch next

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <returns>Events with given ids.</returns>
    let getByIdsAsync (ids: Identity seq) =
        getByIdsCore ids fetch Task.FromResult


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
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx' = Retrieve.getByIdsAsync ids ctx
            match ctx'.Result with
            | Ok events ->
                return events |> Seq.map (fun event -> event.ToEventEntity ())
            | Error error ->
                return raise (error.ToException ())
        }

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <returns>Events with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<int64>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.Id, token)

    /// <summary>
    /// Retrieves information about multiple events in the same project.
    /// A maximum of 1000 events IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="eventIds">The ids of the events to get.</param>
    /// <returns>Events with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<string>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.ExternalId, token)



