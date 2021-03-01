// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk

[<RequireQualifiedAccess>]
module DataSets =
    [<Literal>]
    let Url = "/datasets"

    /// <summary>
    /// Create new data sets in the given project.
    /// </summary>
    /// <param name="assets">The data sets to create.</param>
    /// <returns>List of created data sets.</returns>
    let create (items: DataSetCreate seq) : HttpHandler<unit, DataSet seq> =
        withLogMessage "DataSets:create"
        >=> create items Url

    /// <summary>
    /// Retrieves list of data sets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of data sets matching given filters and optional cursor</returns>
    let list (query: DataSetQuery) : HttpHandler<unit, ItemsWithCursor<#DataSet>> =
        withLogMessage "DataSets:list"
        >=> list query Url

    /// <summary>
    /// Retrieves count of data sets matching filter
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>Count of data sets matching given filters</returns>
    let aggregate (query: DataSetQuery) : HttpHandler<unit, int> =
        withLogMessage "DataSets:aggregate"
        >=> aggregate query Url