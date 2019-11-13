// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk.Sequences
open CogniteSdk

type SequenceSearch =
    private
    | CaseName of string
    | CaseDescription of string
    | CaseQuery of string
    /// Fuzzy search on name
    static member Name name = CaseName name
    /// Fuzzy search on description
    static member Description description = CaseDescription description
    // Search on name and description using wildcard search on each of
    // the words (separated by spaces). Retrieves results where at least
    // one word must match. Example: 'some other'
    static member Query query = CaseQuery query
    static member Render (this: SequenceSearch) =
        match this with
        | CaseName name -> "name", Encode.string name
        | CaseDescription desc -> "description", Encode.string desc
        | CaseQuery query -> "query", Encode.string query

/// The functional Sequence search core module
[<RequireQualifiedAccess>]
module Search =
    [<Literal>]
    let Url = "/sequences/search"

    type SearchSequencesRequest = {
        Limit: int
        Filters: SequenceFilter seq
        Options: SequenceSearch seq
    } with
        member this.Encoder =
            Encode.object [
                if not (Seq.isEmpty this.Filters) then
                    yield "filter", Encode.object [
                        yield! this.Filters |> Seq.map SequenceFilter.Render
                    ]
                if not (Seq.isEmpty this.Options) then
                    yield "search", Encode.object [
                        yield! this.Options |> Seq.map SequenceSearch.Render
                    ]
                if this.Limit > 0 then
                    yield "limit", Encode.int this.Limit
            ]

    let searchCore (limit: int) (options: SequenceSearch seq) (filters: SequenceFilter seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : SearchSequencesRequest = {
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
        >=> json SequenceItemsReadDto.Decoder
        >=> map (fun sequences -> sequences.Items)

    /// <summary>
    /// Retrieves a list of Sequences matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of Sequences matching given criteria.</returns>
    let search (limit: int) (options: SequenceSearch seq) (filters: SequenceFilter seq) (next: NextFunc<SequenceReadDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        searchCore limit options filters fetch next

    /// <summary>
    /// Retrieves a list of Sequences matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of Sequences matching given criteria.</returns>
    let searchAsync (limit: int) (options: SequenceSearch seq) (filters: SequenceFilter seq): HttpContext -> HttpFuncResult<SequenceReadDto seq> =
        searchCore limit options filters fetch finishEarly

[<Extension>]
type SearchSequencesClientExtensions =
    /// <summary>
    /// Retrieves a list of Sequences matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of Sequences matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: SequenceSearch seq, filters: SequenceFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Search.searchAsync limit options filters ctx
            match result with
            | Ok ctx ->
                let sequences = ctx.Response
                return sequences |> Seq.map (fun sequence -> sequence.ToSequenceEntity ())
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }


    /// <summary>
    /// Retrieves a list of Sequences matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    ///
    /// <returns>List of Sequences matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: SequenceSearch seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        let filter = ResizeArray<SequenceFilter>()
        this.SearchAsync(limit, options, filter, token)

    /// <summary>
    /// Retrieves a list of Sequences matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of Sequences matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, filters: SequenceFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        let options = ResizeArray<SequenceSearch>()
        this.SearchAsync(limit, options, filters, token)

