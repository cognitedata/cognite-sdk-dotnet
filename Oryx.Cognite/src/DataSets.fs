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
    /// <param name="items">The data sets to create.</param>
    /// <returns>List of created data sets.</returns>
    let create (items: DataSetCreate seq) : IHttpHandler<unit, DataSet seq> =
        withLogMessage "DataSets:create"
        >=> create items Url

    /// <summary>
    /// Retrieves list of data sets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of data sets matching given filters and optional cursor</returns>
    let list (query: DataSetQuery) : IHttpHandler<unit, ItemsWithCursor<#DataSet>> =
        withLogMessage "DataSets:list"
        >=> list query Url

    /// <summary>
    /// Retrieves count of data sets matching filter
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>Count of data sets matching given filters</returns>
    let aggregate (query: DataSetQuery) : IHttpHandler<unit, int> =
        withLogMessage "DataSets:aggregate"
        >=> aggregate query Url

    /// <summary>
    /// Retrieves information about multiple data sets in the same project. A maximum of 1000 data set IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the data sets to get.</param>
    /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found.</param>
    /// <returns>Data sets with given ids.</returns>
    let retrieve (ids: Identity seq) (ignoreUnknownIds: Nullable<bool>) : IHttpHandler<unit, #DataSet seq> =
        withLogMessage "DataSets:retrieve"
        >=> retrieveIgnoreUnkownIds ids (Option.ofNullable ignoreUnknownIds) Url

    /// <summary>
    /// Update one or more data sets. Supports partial updates, meaning that fields omitted from the requests are not changed.
    /// Returns list of updated data sets.
    /// </summary>
    /// <param name="query">Data set updates to perform.</param>
    /// <returns>List of updated data sets</returns>
    let update (query: IEnumerable<UpdateItem<DataSet>>) : IHttpHandler<unit, #DataSet seq> =
        withLogMessage "DataSets:update"
        >=> update query Url