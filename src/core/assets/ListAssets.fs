// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Assets

open System.Net.Http

open Oryx
open Oryx.SystemTextJson
open Oryx.Cognite
open Oryx.SystemTextJson.ResponseReader

open CogniteSdk.Types
open CogniteSdk.Types.Assets

type AssetItemsReadDto = Common.ResourceItemsWithCursor<AssetReadDto>

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let Url = "/assets/list"

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (options: AssetQuery seq) : HttpHandler<HttpResponseMessage, AssetItemsReadDto, 'a> =
        POST
        >=> setVersion V10
        >=> setContent (new JsonPushStreamContent<AssetQuery seq>(options))
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json None

