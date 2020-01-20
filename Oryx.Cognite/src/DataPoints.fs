// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Collections.Generic
open System.Net.Http

open Com.Cognite.V1.Timeseries.Proto
open Oryx.Cognite

open CogniteSdk
open CogniteSdk.DataPoints
open CogniteSdk.TimeSeries


/// Various time series data points HTTP handlers

[<RequireQualifiedAccess>]
module DataPoints =
    [<Literal>]
    let Url = "/timeseries/data"

    /// Retrieves a list of numeric data points from multiple time series in a project.
    let list (query: DataPointsQuery) : HttpHandler<HttpResponseMessage, DataPointListResponse, 'a> =
        listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Retrieves a list of aggregate data points from multiple time series in a project.
    let listAggregates (query: DataPointsQuery) : HttpHandler<HttpResponseMessage, DataPointListResponse, 'a> =
        listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Create one or more new times eries. Returns a list of created time series.
    let create (items: DataPointsWriteType seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        createProtobuf items Url

    /// Delete data points from 1 or more (multiple) time series.
    let delete (items: IEnumerable<DataPointsDeleteType>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete items Url

    /// Retrieves the latest data point in multiple time series in the same project.
    let latest (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<TimeSeriesReadDto>, 'a> =
        retrieve ids Url
