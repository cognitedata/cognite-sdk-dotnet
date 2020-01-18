// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk
open CogniteSdk.Assets

/// Various asset HTTP handlers.

[<RequireQualifiedAccess>]
module Assets =
    [<Literal>]
    let Url = "/assets"

    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    let get (assetId: int64) : HttpHandler<HttpResponseMessage, AssetReadDto, 'a> =
        getById assetId Url

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (query: AssetQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithCursor<AssetReadDto>, 'a> =
        list query Url

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let create (items: IEnumerable<AssetWriteDto>) : HttpHandler<HttpResponseMessage, IEnumerable<AssetReadDto>, 'a> =
        create items Url

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (assets: AssetDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete assets Url

    /// <summary>
    /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <returns>Assets with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<AssetReadDto>, 'a> =
        retrieve ids Url

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Asset search query.</param>
    /// <returns>List of assets matching given criteria.</returns>
    let search (query: SearchQueryDto<AssetFilterDto, SearchDto>) : HttpHandler<HttpResponseMessage, IEnumerable<AssetReadDto>, 'a> =
        search query Url

    /// <summary>
    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="assets">The list of assets to update.</param>
    /// <returns>List of updated assets.</returns>
    let update (query: IEnumerable<UpdateItemType<AssetUpdateDto>>) : HttpHandler<HttpResponseMessage, IEnumerable<AssetReadDto>, 'a>  =
        update query Url

