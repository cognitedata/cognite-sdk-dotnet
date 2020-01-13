// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.SystemTextJson.ResponseReader
open Oryx.Cognite
open Oryx.SystemTextJson

open CogniteSdk
open CogniteSdk.TimeSeries

// Various asset item types

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
        let url = Url +/ "list"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<TimeSeriesQuery>(query, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created timeseries.</returns>

    let create (items: ItemsWithoutCursor<TimeSeriesWriteDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a> =
        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (new JsonPushStreamContent<ItemsWithoutCursor<TimeSeriesWriteDto>>(items, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    let delete (items: TimeSeriesDeleteDto) : HttpHandler<HttpResponseMessage, HttpResponseMessage, 'a> =
        let url = Url +/ "delete"

        POST
        >=> setVersion V10
        >=> setContent (new JsonPushStreamContent<TimeSeriesDeleteDto>(items))
        >=> setResource url
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <returns>The timeseries with the given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a> =
        let request = ItemsWithoutCursor<Identity>(Items = ids)
        let url = Url +/ "byids"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<ItemsWithoutCursor<Identity>>(request, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Time series search query.</param>
    /// <returns>List of time series matching given criteria.</returns>
    let search (query: TimeSeriesSearchQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a> =
        let url = Url +/ "search"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<TimeSeriesSearchQueryDto>(query, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    /// <summary>
    /// Updates multiple timeseries within the same project.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed
    /// <param name="timeseries">List of tuples of timeseries id to update and updates to perform on that timeseries.</param>
    /// <returns>List of updated timeseries.</returns>
    let update (query: ItemsWithoutCursor<UpdateItem<TimeSeriesUpdateDto>>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TimeSeriesReadDto>, 'a>  =
        let url = Url +/ "update"

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<ItemsWithoutCursor<UpdateItem<TimeSeriesUpdateDto>>>(query, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

