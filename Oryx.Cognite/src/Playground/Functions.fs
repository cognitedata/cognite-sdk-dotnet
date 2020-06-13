// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Playground

open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Playground

open System.Collections.Generic
open CogniteSdk
open CogniteSdk.Types.Common

/// Various asset HTTP handlers.

[<RequireQualifiedAccess>]
module Functions =
    [<Literal>]
    let Url = "/functions"

    /// <summary>
    /// Retrieves list of functions.
    /// </summary>
    /// <returns>List of Functions.</returns>
    let list () : HttpHandler<HttpResponseMessage, ItemsWithCursor<Function>, 'a> =
        withLogMessage "Functions:list"
        >=> getPlayground Url

    /// <summary>
    /// Create new Functions in the given project.
    /// </summary>
    /// <param name="functions">The Functions to create.</param>
    /// <returns>List of created Functions.</returns>
    let create (functions: FunctionCreate seq) : HttpHandler<HttpResponseMessage, Function seq, 'a> =
        withLogMessage "Functions:create"
        >=> create functions Url

    /// <summary>
    /// Delete multiple Functions in the same project.
    /// </summary>
    /// <param name="ids">The list of ids for Functions to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (ids: Identity seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let items = ItemsWithoutCursor(Items=ids)
        withLogMessage "Functions:delete"
        >=> delete items Url

    /// <summary>
    /// Retrieves information about multiple Functions in the same project. A maximum of 1000 Functions IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the Functions to get.</param>
    /// <returns>Functions with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, Function seq, 'a> =
        withLogMessage "Functions:retrieve"
        >=> retrieve ids Url
