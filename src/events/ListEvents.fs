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

open CogniteSdk
open Oryx.Decode


type EventQuery =
    private
    | CaseLimit of int
    | CaseCursor of string

    /// Max number of results to return
    static member Limit limit =
        if limit > MaxLimitSize || limit < 1 then
            failwith "Limit must be set to 1000 or less"
        CaseLimit limit
    /// Cursor return from previous request
    static member Cursor cursor = CaseCursor cursor

    static member Render (this: EventQuery) =
        match this with
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseCursor cursor -> "cursor", Encode.string cursor

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let Url = "/events/list"

    type Request = {
        Filters : EventFilter seq
        Options : EventQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map EventFilter.Render
                ]
                yield! this.Options |> Seq.map EventQuery.Render
            ]

    let listCore (options: EventQuery seq) (filters: EventFilter seq)(fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : Request = {
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

    /// <summary>
    /// Retrieves list of events matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of events matching given filters and optional cursor</returns>
    let list (options: EventQuery seq) (filters: EventFilter seq) (next: NextFunc<EventItemsReadDto,'a>)
        : HttpContext -> HttpFuncResult<'a> =
            listCore options filters fetch next

    /// <summary>
    /// Retrieves list of events matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of events matching given filters and optional cursor</returns>
    let listAsync (options: EventQuery seq) (filters: EventFilter seq)
        : HttpContext -> HttpFuncResult<EventItemsReadDto> =
            listCore options filters fetch finishEarly


[<Extension>]
type ListEventsExtensions =
    /// <summary>
    /// Retrieves list of events matching query, filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of events matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: EventQuery seq, filters: EventFilter seq, [<Optional>] token: CancellationToken) : Task<EventItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listAsync options filters ctx
            match result with
            | Ok ctx ->
                let events = ctx.Response
                let cursor = if events.NextCursor.IsSome then events.NextCursor.Value else Unchecked.defaultof<string>
                let items : EventItems = {
                        NextCursor = cursor
                        Items = events.Items |> Seq.map (fun event -> event.ToEventEntity ())
                    }
                return items
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

    /// <summary>
    /// Retrieves list of events matching filter.
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of events matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, filters: EventFilter seq, [<Optional>] token: CancellationToken) : Task<EventItems> =
        let query = ResizeArray<EventQuery>()
        this.ListAsync(query, filters)
