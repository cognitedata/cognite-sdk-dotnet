// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Assets

open Oryx
open Oryx.SystemTextJson
open Oryx.Cognite

open CogniteSdk.Types.Assets

[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/assets/delete"

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    /// <param name="next">Async handler to use</param>
    let delete (assets: AssetDeleteDto) =
        POST
        >=> setVersion V10
        >=> setContent (new JsonPushStreamContent<AssetDeleteDto>(assets))
        >=> setResource Url
        >=> fetch
        >=> withError decodeError

