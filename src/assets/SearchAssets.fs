// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk.Assets
open CogniteSdk

type AssetSearch =
    private
    | CaseName of string
    | CaseDescription of string

    /// Fuzzy search on name
    static member Name name = CaseName name
    /// Fuzzy search on description
    static member Description description = CaseDescription description

    static member Render (this: AssetSearch) =
        match this with
        | CaseName name -> "name", Encode.string name
        | CaseDescription desc -> "description", Encode.string desc

/// The functional asset search core module
[<RequireQualifiedAccess>]
module Search =
    [<Literal>]
    let Url = "/assets/search"

    type SearchAssetsRequest = {
        Limit: int
        Filters: AssetFilter seq
        Options: AssetSearch seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map AssetFilter.Render
                ]
                yield "search", Encode.object [
                    yield! this.Options |> Seq.map AssetSearch.Render
                ]
                if this.Limit > 0 then
                    yield "limit", Encode.int this.Limit
            ]

    let searchCore (limit: int) (options: AssetSearch seq) (filters: AssetFilter seq)(fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse AssetItemsReadDto.Decoder (fun assets -> assets.Items)
        let request : SearchAssetsRequest = {
            Limit = limit
            Filters = filters
            Options = options
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let search (limit: int) (options: AssetSearch seq) (filters: AssetFilter seq) (next: NextFunc<AssetReadDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        searchCore limit options filters fetch next

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let searchAsync (limit: int) (options: AssetSearch seq) (filters: AssetFilter seq): HttpContext -> HttpFuncResult<AssetReadDto seq> =
        searchCore limit options filters fetch finishEarly

[<Extension>]
type SearchAssetsClientExtensions =
    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: AssetSearch seq, filters: AssetFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Search.searchAsync limit options filters ctx
            match result with
            | Ok ctx ->
                let assets = ctx.Response
                return assets |> Seq.map (fun asset -> asset.ToAssetEntity ())
            | Error error ->
                return raise (error.ToException ())
        }


    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: AssetSearch seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        let filter = ResizeArray<AssetFilter>()
        this.SearchAsync(limit, options, filter, token)

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, filters: AssetFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        let options = ResizeArray<AssetSearch>()
        this.SearchAsync(limit, options, filters, token)

