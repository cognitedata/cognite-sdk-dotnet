// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk
open CogniteSdk.TimeSeries

/// Various time series HTTP handlers
[<RequireQualifiedAccess>]
module TimeSeries =
    [<Literal>]
    let Url = "/timeseries"

    /// Retrieves list of time series matching filter, and a cursor if given limit is exceeded. Returns list of time
    /// series matching given filters and optional cursor.
    let list (query: TimeSeriesQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithCursor<TimeSeriesReadDto>, 'a> =
        list query Url
        >=> logWithMessage "TimeSeries:get"

    /// Create one or more new time series. Returns list of created time series.
    let create (items: IEnumerable<TimeSeriesWriteDto>) : HttpHandler<HttpResponseMessage, IEnumerable<TimeSeriesReadDto>, 'a> =
        create items Url
        >=> logWithMessage "TimeSeries:create"

    /// Delete one or more time series.
    let delete (items: TimeSeriesDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete items Url
        >=> logWithMessage "TimeSeries:delete"

    /// Retrieves information about multiple time series in the same project. A maximum of 1000 time series IDs may be
    /// listed per request and all of them must be unique. Returns the time series with the given ids.
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, TimeSeriesReadDto seq, 'a> =
        retrieve ids Url
        >=> logWithMessage "TimeSeries:retrieve"

    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination. Returns
    /// list of time series matching given criteria.</returns>
    let search (query: TimeSeriesSearchDto) : HttpHandler<HttpResponseMessage, TimeSeriesReadDto seq, 'a> =
        search query Url
        >=> logWithMessage "TimeSeries:search"

    /// Updates multiple time series within the same project. This operation supports partial updates, meaning that
    /// fields omitted from the requests are not changed Returns list of updated time series.
    let update (query: IEnumerable<UpdateItem<TimeSeriesUpdateDto>>) : HttpHandler<HttpResponseMessage, TimeSeriesReadDto seq, 'a>  =
        update query Url
        >=> logWithMessage "TimeSeries:update"

