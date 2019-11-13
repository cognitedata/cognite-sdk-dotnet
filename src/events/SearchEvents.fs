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
open Thoth.Json.Net

open CogniteSdk.Events
open CogniteSdk

type EventSearch =
    private
    | CaseDescription of string

    /// Fuzzy search on description
    static member Description description = CaseDescription description

    static member Render (this: EventSearch) =
        match this with
        | CaseDescription desc -> "description", Encode.string desc


/// The functional event search core module
[<RequireQualifiedAccess>]
module Search =
    [<Literal>]
    let Url = "/events/search"

    type Events = {
        Items: EventReadDto seq
    } with
        static member Decoder : Decoder<Events> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list EventReadDto.Decoder |> Decode.map seq)
            })

    type SearchEventsRequest = {
        Limit: int
        Filters: EventFilter seq
        Options: EventSearch seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map EventFilter.Render
                ]
                yield "search", Encode.object [
                    yield! this.Options |> Seq.map EventSearch.Render
                ]
                if this.Limit > 0 then
                    yield "limit", Encode.int this.Limit
            ]

    let searchCore (limit: int) (options: EventSearch seq) (filters: EventFilter seq)(fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : SearchEventsRequest = {
            Limit = limit
            Filters = filters
            Options = options
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json EventItemsReadDto.Decoder
        >=> map (fun events -> events.Items)

    /// <summary>
    /// Retrieves a list of events matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of events matching given criteria.</returns>
    let search (limit: int) (options: EventSearch seq) (filters: EventFilter seq) (next: NextFunc<EventReadDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        searchCore limit options filters fetch next

    /// <summary>
    /// Retrieves a list of events matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of events matching given criteria.</returns>
    let searchAsync (limit: int) (options: EventSearch seq) (filters: EventFilter seq): HttpContext -> HttpFuncResult<EventReadDto seq> =
        searchCore limit options filters fetch finishEarly

[<Extension>]
type SearchEventsClientExtensions =
    /// <summary>
    /// Retrieves a list of events matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of events matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: EventSearch seq, filters: EventFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Search.searchAsync limit options filters ctx
            match result with
            | Ok ctx ->
                return ctx.Response |> Seq.map (fun event -> event.ToEventEntity ())
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }
