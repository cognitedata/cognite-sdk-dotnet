// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading

open Oryx
open Thoth.Json.Net

open CogniteSdk
open System.Threading.Tasks
open FSharp.Control.Tasks.V2.ContextInsensitive


type SequenceQuery =
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

    static member Render (this: SequenceQuery) =
        match this with
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseCursor cursor -> "cursor", Encode.string cursor

type RowQuery =
    private
    | CaseStart of int64
    | CaseEnd of int64
    | CaseLimit of int32
    | CaseCursor of string
    | CaseColumns of string
    | CaseId of Identity

    /// Lowest row number included
    static member Start start = CaseStart start
    /// Get rows up to, but excluding, this row number
    static member End e = CaseEnd e
    /// Maximum number of rows returned in one request
    static member Limit limit =
        if limit > MaxLimitSize || limit < 1 then
            failwith "Limit must be set to 1000 or less"
        CaseLimit limit
    /// Cursor for pagination returned from a previous request. Apart
    /// From this cursor, the rest of the request object should be the same as for the original request.
    static member Cursor cursor = CaseCursor cursor
    /// Columns to be included. Specified as list of column externalIds. In case this filter is not set, all
    /// available columns will be returned.
    static member Columns columns = CaseColumns columns
    /// A server-generated ID for the object.
    static member Id identity = CaseId identity

    static member Render (this: RowQuery) =
        match this with
        | CaseStart start -> "start", Encode.int53 start
        | CaseEnd e -> "end", Encode.int53 e
        | CaseColumns cols -> "columns", Encode.string cols
        | CaseId identity -> identity.Render
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseCursor cursor -> "cursor", Encode.string cursor

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let Url = "/sequences/list"
    let dataUrl = "/sequences/data/list"

    type Request = {
        Filters : SequenceFilter seq
        Options : SequenceQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map SequenceFilter.Render
                ]
                yield! this.Options |> Seq.map SequenceQuery.Render
            ]

    type DataRequest = {
        Options : RowQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield! this.Options |> Seq.map RowQuery.Render
            ]

    let listRowsCore (options: RowQuery seq)  (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : DataRequest = {
            Options = options
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource dataUrl
        >=> fetch
        >=> withError decodeError
        >=> json SequenceDataReadDto.Decoder

    let listCore (options: SequenceQuery seq) (filters: SequenceFilter seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
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
        >=> json SequenceItemsReadDto.Decoder

    /// <summary>
    /// Retrieves list of Sequences matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of Sequences matching given filters and optional cursor</returns>
    let list (options: SequenceQuery seq) (filters: SequenceFilter seq) (next: NextFunc<SequenceItemsReadDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        listCore options filters fetch next

    /// <summary>
    /// Retrieves list of Sequences matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of Sequences matching given filters and optional cursor</returns>
    let listAsync (options: SequenceQuery seq) (filters: SequenceFilter seq) : HttpContext -> HttpFuncResult<SequenceItemsReadDto> =
        listCore options filters fetch finishEarly

    /// <summary>
    /// Retrieves Sequence data with columns and rows matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>Sequences data with rows and columns matching given filters and optional cursor</returns>
    let listRows (options: RowQuery seq) (next: NextFunc<SequenceDataReadDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        listRowsCore options fetch next

    /// <summary>
    /// Retrieves Sequence data with columns and rows matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <returns>Sequences data with rows and columns matching given filters and optional cursor</returns>
    let listRowsAsync (options: RowQuery seq) : HttpContext -> HttpFuncResult<SequenceDataReadDto> =
        listRowsCore options fetch finishEarly

[<Extension>]
type ListSequencesExtensions =
    /// <summary>
    /// Retrieves list of Sequences matching query, filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of Sequences matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: SequenceQuery seq, filters: SequenceFilter seq, [<Optional>] token: CancellationToken) : Task<SequenceItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listAsync options filters ctx
            match result with
            | Ok ctx ->
                let sequences = ctx.Response
                let cursor = sequences.NextCursor |> Option.defaultValue Unchecked.defaultof<string>
                let items : SequenceItems = {
                    NextCursor = cursor
                    Items = sequences.Items |> Seq.map (fun sequence -> sequence.ToSequenceEntity ())
                }
                return items
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves list of Sequences matching filter.
    /// </summary>
    /// <param name="filters">Search filters</param>
    /// <returns>List of Sequences matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, filters: SequenceFilter seq, [<Optional>] token: CancellationToken) : Task<SequenceItems> =
        let query = ResizeArray<SequenceQuery>()
        this.ListAsync(query, filters, token)

    /// <summary>
    /// Retrieves list of Sequences with a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <returns>List of Sequences matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: SequenceQuery seq, [<Optional>] token: CancellationToken) : Task<SequenceItems> =
        let filter = ResizeArray<SequenceFilter>()
        this.ListAsync(options, filter, token)
