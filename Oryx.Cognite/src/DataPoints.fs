// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open System.Collections.Generic
open System.Collections.Generic
open CogniteSdk
open CogniteSdk.DataPoints
open CogniteSdk.TimeSeries

/// Various time series HTTP handlers

[<RequireQualifiedAccess>]
module DataPoints =
    [<Literal>]
    let Url = "/timeseries/data"

    /// Retrieves a list of data points from multiple time series in a project. This operation supports aggregation, but not pagination.
    let list (query: DataPointsQuery) : HttpHandler<HttpResponseMessage, DataPointsReadDto<DataPointNumericDto>, 'a> =
        list query Url
    
    /// Retrieves a list of data points from multiple time series in a project. This operation supports aggregation, but not pagination.
    let listStrings (query: DataPointsQuery) : HttpHandler<HttpResponseMessage, DataPointsReadDto<DataPointStringDto>, 'a> =
        Handler.list query Url

    /// Retrieves a list of data points from multiple time series in a project. This operation supports aggregation, but not pagination.
    let listAggregates (query: DataPointsQuery) : HttpHandler<HttpResponseMessage, DataPointsReadDto<DataPointAggregateDto>, 'a> =
        Handler.list query Url

    /// <summary>
    /// Create one or more new times eries.
    /// </summary>
    /// <param name="items">The list of time series to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created time series.</returns>

    let create (items: IEnumerable<TimeSeriesWriteDto>) : HttpHandler<HttpResponseMessage, IEnumerable<TimeSeriesReadDto>, 'a> =
        create items Url

    /// <summary>
    /// Delete one or more time series.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    let delete (items: TimeSeriesDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete items Url

    /// <summary>
    /// Retrieves information about multiple time series in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the time series to get.</param>
    /// <returns>The time series with the given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<TimeSeriesReadDto>, 'a> =
        retrieve ids Url
