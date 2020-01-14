// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open CogniteSdk
open CogniteSdk.TimeSeries

/// Various time series HTTP handlers

[<RequireQualifiedAccess>]
module TimeSeries =
    [<Literal>]
    let Url = "/timeseries"

    /// <summary>
    /// Retrieves list of time series matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of time series matching given filters and optional cursor</returns>
    let list (query: TimeSeriesQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<TimeSeriesReadDto>, 'a> =
        filter query Url

    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created timeseries.</returns>

    let create (items: ItemsWithoutCursor<TimeSeriesWriteDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a> =
        create items Url

    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    let delete (items: TimeSeriesDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete items Url

    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <returns>The timeseries with the given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a> =
        retrieve ids Url

    /// <summary>
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Time series search query.</param>
    /// <returns>List of time series matching given criteria.</returns>
    let search (query: SearchQueryDto<TimeSeriesFilterDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a> =
        search query Url

    /// <summary>
    /// Updates multiple timeseries within the same project.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed
    /// <param name="timeseries">List of tuples of timeseries id to update and updates to perform on that timeseries.</param>
    /// <returns>List of updated timeseries.</returns>
    let update (query: ItemsWithoutCursor<UpdateItem<TimeSeriesUpdateDto>>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a>  =
        update query Url

