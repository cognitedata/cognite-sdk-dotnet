// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
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
    let get (assetId: int64) : IHttpHandler<unit, #Asset> =
        withLogMessage "Assets:get"
        >=> getById assetId Url

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (query: AssetQuery) : IHttpHandler<unit, ItemsWithCursor<#Asset>> =
        withLogMessage "Assets:list"
        >=> list query Url

    /// <summary>
    /// Retrieves count of assets matching filter
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>Count of assets matching given filters</returns>
    let aggregate (query: AssetQuery) : IHttpHandler<unit, int> =
        withLogMessage "Assets:aggregate"
        >=> aggregate query Url

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="items">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let create (items: AssetCreate seq) : IHttpHandler<unit, Asset seq> =
        withLogMessage "Assets:create"
        >=> create items Url

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (assets: AssetDelete) : IHttpHandler<unit, EmptyResponse> =
        withLogMessage "Assets:delete"
        >=> delete assets Url

    /// <summary>
    /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the assets to get.</param>
    /// <param name="ignoreUnknownIds">True if unknown ids should be ignored.</param>
    /// <returns>Assets with given ids.</returns>
    let retrieve (ids: Identity seq) (ignoreUnknownIds: Nullable<bool>) : IHttpHandler<unit, #Asset seq> =
        withLogMessage "Assets:retrieve"
        >=> retrieveIgnoreUnkownIds ids (Option.ofNullable ignoreUnknownIds) Url

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Asset search query.</param>
    /// <returns>List of assets matching given criteria.</returns>
    let search (query: AssetSearch) : IHttpHandler<unit, #Asset seq> =
        withLogMessage "Assets:search"
        >=> search query Url

    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated assets.
    let update (query: IEnumerable<UpdateItem<AssetUpdate>>) : IHttpHandler<unit, #Asset seq>  =
        withLogMessage "Assets:update"
        >=> update query Url

