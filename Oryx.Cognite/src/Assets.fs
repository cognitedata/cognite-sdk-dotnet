// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk

/// Various asset HTTP handlers.

[<RequireQualifiedAccess>]
module Assets =
    [<Literal>]
    let Url = "/assets"

    /// Retrieves information about an asset given an asset id. Returns asset with the given id.
    let get (assetId: int64) : HttpHandler<HttpResponseMessage, Asset, 'a> =
        getById assetId Url
        >=> logWithMessage "Assets:get"

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (query: AssetQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<Asset>, 'a> =
        list query Url
        >=> logWithMessage "Assets:list"

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let create (items: AssetCreate seq) : HttpHandler<HttpResponseMessage, Asset seq, 'a> =
        create items Url
        >=> logWithMessage "Assets:create"

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (assets: AssetDelete) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete assets Url
        >=> logWithMessage "Assets:delete"

    /// <summary>
    /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <returns>Assets with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, Asset seq, 'a> =
        retrieve ids Url
        >=> logWithMessage "Assets:retrieve"

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Asset search query.</param>
    /// <returns>List of assets matching given criteria.</returns>
    let search (query: AssetSearch) : HttpHandler<HttpResponseMessage, Asset seq, 'a> =
        search query Url
        >=> logWithMessage "Assets:search"

    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated assets.
    let update (query: IEnumerable<UpdateItem<AssetUpdate>>) : HttpHandler<HttpResponseMessage, Asset seq, 'a>  =
        update query Url
        >=> logWithMessage "Assets:update"

