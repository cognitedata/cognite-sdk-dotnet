// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various time series HTTP handlers
[<RequireQualifiedAccess>]
module TimeSeries =
    [<Literal>]
    let Url = "/timeseries"

    /// Retrieves information about an time series given an time series id. Returns asset with the given id.
    let get (tsId: int64) : HttpHandler<HttpResponseMessage, #TimeSeries, 'TResult> =
        withLogMessage "TimeSeries:get"
        >=> retrieve [ Identity tsId ] Url
        >=> map Seq.head

    /// Retrieves list of time series matching filter, and a cursor if given limit is exceeded. Returns list of time
    /// series matching given filters and optional cursor.
    let list (query: TimeSeriesQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<#TimeSeries>, 'TResult> =
        withLogMessage "TimeSeries:get"
        >=> list query Url

    /// Retrieves number of time series matching filter. Returns number of time
    /// series matching given filters.
    let aggregate (query: TimeSeriesQuery) : HttpHandler<HttpResponseMessage, int32, 'TResult> =
        withLogMessage "TimeSeries:aggregate"
        >=> aggregate query Url

    /// Create one or more new time series. Returns list of created time series.
    let create (items: IEnumerable<TimeSeriesCreate>) : HttpHandler<HttpResponseMessage, IEnumerable<#TimeSeries>, 'TResult> =
        withLogMessage "TimeSeries:create"
        >=> create items Url

    /// Delete one or more time series.
    let delete (items: TimeSeriesDelete) : HttpHandler<HttpResponseMessage, EmptyResponse, 'TResult> =
        withLogMessage "TimeSeries:delete"
        >=> delete items Url

    /// Retrieves information about multiple time series in the same project. A maximum of 1000 time series IDs may be
    /// listed per request and all of them must be unique. Returns the time series with the given ids.
    let retrieve (ids: Identity seq) (ignoreUnknownIds: Nullable<bool>) : HttpHandler<HttpResponseMessage, #TimeSeries seq, 'TResult> =
        withLogMessage "TimeSeries:retrieve"
        >=> retrieveIgnoreUnkownIds ids (Option.ofNullable ignoreUnknownIds) Url

    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination. Returns
    /// list of time series matching given criteria.
    let search (query: TimeSeriesSearch) : HttpHandler<HttpResponseMessage, #TimeSeries seq, 'TResult> =
        withLogMessage "TimeSeries:search"
        >=> search query Url

    /// Updates multiple time series within the same project. This operation supports partial updates, meaning that
    /// fields omitted from the requests are not changed Returns list of updated time series.
    let update (query: IEnumerable<UpdateItem<TimeSeriesUpdate>>) : HttpHandler<HttpResponseMessage, #TimeSeries seq, 'TResult>  =
        withLogMessage "TimeSeries:update"
        >=> update query Url

    /// Executes an on-the-fly synthetic query
    /// For example you can use the expression "24 * ts{externalId='production/hour'}" to convert from hourly to daily production rates.
    /// More about synthetic queries https://docs.cognite.com/dev/concepts/resource_types/timeseries.html#synthetic-time-series
    let syntheticQuery (query: TimeSeriesSyntheticQuery) : HttpHandler<HttpResponseMessage, IEnumerable<DataPointsSyntheticItem>, 'TResult> =
        withLogMessage "TimeSeries:syntheticQuery"
        >=> req {
            let url = Url +/ "synthetic/query"
            let! ret = postV10<TimeSeriesSyntheticQuery, ItemsWithoutCursor<DataPointsSyntheticItem>, 'TResult> query url
            return ret.Items
        }
