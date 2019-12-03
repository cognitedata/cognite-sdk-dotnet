// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading

open Oryx
open Oryx.ResponseReaders
open Thoth.Json.Net

open CogniteSdk
open System.Threading.Tasks
open FSharp.Control.Tasks.V2.ContextInsensitive


type AssetQuery =
    private
    | CaseLimit of int
    | CaseCursor of string

    /// Max number of results to return
    static member Limit limit = CaseLimit limit
    /// Cursor return from previous request
    static member Cursor cursor = CaseCursor cursor

    static member Render (this: AssetQuery) =
        match this with
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseCursor cursor -> "cursor", Encode.string cursor

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let Url = "/assets/list"

    type Request = {
        Filters : AssetFilter seq
        Options : AssetQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map AssetFilter.Render
                ]
                yield! this.Options |> Seq.map AssetQuery.Render
            ]

    let listCore (options: AssetQuery seq) (filters: AssetFilter seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
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
        >=> json AssetItemsReadDto.Decoder

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (options: AssetQuery seq) (filters: AssetFilter seq) (next: NextFunc<AssetItemsReadDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        listCore options filters fetch next

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let listAsync (options: AssetQuery seq) (filters: AssetFilter seq) : HttpContext -> HttpFuncResult<AssetItemsReadDto> =
        listCore options filters fetch finishEarly

[<Extension>]
type ListAssetsExtensions =
    /// <summary>
    /// Retrieves list of assets matching query, filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: AssetQuery seq, filters: AssetFilter seq, [<Optional>] token: CancellationToken) : Task<AssetItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listAsync options filters ctx
            match result with
            | Ok ctx ->
                let assets = ctx.Response
                let cursor = if assets.NextCursor.IsSome then assets.NextCursor.Value else Unchecked.defaultof<string>
                let items : AssetItems = {
                    NextCursor = cursor
                    Items = assets.Items |> Seq.map (fun asset -> asset.ToAssetEntity ())
                }
                return items
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves list of assets matching filter.
    /// </summary>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, filters: AssetFilter seq, [<Optional>] token: CancellationToken) : Task<AssetItems> =
        let query = ResizeArray<AssetQuery>()
        this.ListAsync(query, filters, token)

    /// <summary>
    /// Retrieves list of assets with a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: AssetQuery seq, [<Optional>] token: CancellationToken) : Task<AssetItems> =
        let filter = ResizeArray<AssetFilter>()
        this.ListAsync(options, filter, token)
