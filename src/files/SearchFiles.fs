// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk.Files
open CogniteSdk

type FileSearch =
    private
    | CaseName of string

    /// Fuzzy search on name
    static member Name name = CaseName name

    static member Render (this: FileSearch) =
        match this with
        | CaseName name -> "name", Encode.string name

/// The functional file search core module
[<RequireQualifiedAccess>]
module Search =
    [<Literal>]
    let Url = "/files/search"

    type SearchFilesRequest = {
        Limit: int
        Filters: FileFilter seq
        Options: FileSearch seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map FileFilter.Render
                ]
                yield "search", Encode.object [
                    yield! this.Options |> Seq.map FileSearch.Render
                ]
                if this.Limit > 0 then
                    yield "limit", Encode.int this.Limit
            ]

    let searchCore (limit: int) (options: FileSearch seq) (filters: FileFilter seq)(fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : SearchFilesRequest = {
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
        >=> json FileItemsReadDto.Decoder
        >=> map (fun files -> files.Items)


    /// <summary>
    /// Retrieves a list of Files matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of Files matching given criteria.</returns>
    let search (limit: int) (options: FileSearch seq) (filters: FileFilter seq) (next: NextFunc<FileReadDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        searchCore limit options filters fetch next

    /// <summary>
    /// Retrieves a list of Files matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of Files matching given criteria.</returns>
    let searchAsync (limit: int) (options: FileSearch seq) (filters: FileFilter seq): HttpContext -> HttpFuncResult<FileReadDto seq> =
        searchCore limit options filters fetch finishEarly

[<Extension>]
type SearchFilesClientExtensions =
    /// <summary>
    /// Retrieves a list of files matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of files matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: FileSearch seq, filters: FileFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Search.searchAsync limit options filters ctx
            match result with
            | Ok ctx ->
                let files = ctx.Response
                return files |> Seq.map (fun file -> file.ToFileEntity ())
            | Error error -> return raiseError error
        }


    /// <summary>
    /// Retrieves a list of files matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    ///
    /// <returns>List of files matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: FileSearch seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        let filter = ResizeArray<FileFilter>()
        this.SearchAsync(limit, options, filter, token)

    /// <summary>
    /// Retrieves a list of files matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of files matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, filters: FileFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        let options = ResizeArray<FileSearch>()
        this.SearchAsync(limit, options, filters, token)

