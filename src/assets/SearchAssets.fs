// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open CogniteSdk.Assets
open Thoth.Json.Net

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

    type Assets = {
        Items: AssetReadDto seq
    } with
        static member Decoder : Decoder<Assets> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
            })

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

    let searchCore (limit: int) (options: AssetSearch seq) (filters: AssetFilter seq)(fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse AssetItemsReadDto.Decoder (fun assets -> assets.Items)
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
        >=> decoder

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let search (limit: int) (options: AssetSearch seq) (filters: AssetFilter seq) (next: NextFunc<AssetReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
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
    let searchAsync (limit: int) (options: AssetSearch seq) (filters: AssetFilter seq): HttpContext -> Async<Context<AssetReadDto seq>> =
        searchCore limit options filters fetch Async.single

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
        async {
            let! ctx = Search.searchAsync limit options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets |> Seq.map (fun asset -> asset.ToAssetEntity ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask (op, cancellationToken = token)


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

