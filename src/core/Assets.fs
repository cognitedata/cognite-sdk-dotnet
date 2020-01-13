// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.SystemTextJson.ResponseReader
open Oryx.Cognite
open Oryx.SystemTextJson

open CogniteSdk
open CogniteSdk.Assets

// Various asset item types

[<RequireQualifiedAccess>]
module Assets =
    [<Literal>]
    let Url = "/assets"

    /// <summary>
    /// Retrieves information about an asset given an asset id. Expects a next continuation handler.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Asset with the given id.</returns>
    let get (assetId: int64) : HttpHandler<HttpResponseMessage, AssetReadDto, 'a> =
        let url = Url +/ sprintf "%d" assetId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (query: AssetQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<AssetReadDto>, 'a> =
        let url = Url +/ "list"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<AssetQuery>(query, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let create (items: ItemsWithoutCursor<AssetWriteDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<AssetReadDto>, 'a> =
        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (new JsonPushStreamContent<ItemsWithoutCursor<AssetWriteDto>>(items, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    /// <param name="next">Async handler to use</param>
    let delete (assets: AssetDeleteDto) : HttpHandler<HttpResponseMessage, HttpResponseMessage, 'a> =
        let url = Url +/ "delete"

        POST
        >=> setVersion V10
        >=> setContent (new JsonPushStreamContent<AssetDeleteDto>(assets))
        >=> setResource url
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <returns>Assets with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<AssetReadDto>, 'a> =
        let request = ItemsWithoutCursor<Identity>(Items = ids)
        let url = Url +/ "byids"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<ItemsWithoutCursor<Identity>>(request, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="query">Asset search query.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let search (query: AssetSearchQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<AssetReadDto>, 'a> =
        let url = Url +/ "search"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<AssetSearchQueryDto>(query, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="assets">The list of assets to update.</param>
    /// <returns>List of updated assets.</returns>
    let update (query: ItemsWithoutCursor<UpdateItem<AssetUpdateDto>>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<AssetReadDto>, 'a>  =
        let url = Url +/ "update"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<ItemsWithoutCursor<UpdateItem<AssetUpdateDto>>>(query, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

