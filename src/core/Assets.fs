// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Assets

open System.Net.Http

open Oryx
open Oryx.SystemTextJson.ResponseReader
open Oryx.Cognite
open Oryx.SystemTextJson

open CogniteSdk.Types.Assets
open CogniteSdk.Types

type AssetItemsWithCursorReadDto = Common.ResourceItemsWithCursor<AssetReadDto>
type AssetItemsReadDto = Common.ResourceItems<AssetReadDto>
type AssetItemsWriteDto = Common.ResourceItems<AssetWriteDto>

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
        let url = Url + sprintf "/%d" assetId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> withError decodeError
        >=> json None

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (options: AssetQuery seq) : HttpHandler<HttpResponseMessage, AssetItemsWithCursorReadDto, 'a> =
        POST
        >=> setVersion V10
        >=> setContent (new JsonPushStreamContent<AssetQuery seq>(options))
        >=> setResource "/assets/list"
        >=> fetch
        >=> withError decodeError
        >=> json (Some jsonOptions)

    let create (assets: AssetItemsWriteDto) : HttpHandler<HttpResponseMessage, AssetItemsReadDto, 'a> =
        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (new JsonPushStreamContent<AssetItemsWriteDto>(assets))
        >=> fetch
        >=> withError decodeError
        >=> json (Some jsonOptions)
