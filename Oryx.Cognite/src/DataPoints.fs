// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Collections.Generic
open System.Net.Http

open Com.Cognite.V1.Timeseries.Proto
open Oryx
open Oryx.Cognite

open CogniteSdk


/// Various time series data points HTTP handlers

[<RequireQualifiedAccess>]
module DataPoints =
    [<Literal>]
    let Url = "/timeseries/data"

    /// Retrieves a list of numeric data points from multiple time series in a project.
    let list (query: DataPointsQuery) : IHttpHandler<unit, DataPointListResponse> =
        withLogMessage "DataPoints:list"
        >=> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Retrieves a list of aggregate data points from multiple time series in a project.
    let listAggregates (query: DataPointsQuery) : IHttpHandler<unit, DataPointListResponse> =
        withLogMessage "DataPoints:listAggregates"
        >=> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Create one or more new times eries. Returns a list of created time series.
    let create (items: DataPointInsertionRequest) : IHttpHandler<unit, EmptyResponse> =
        withLogMessage "DataPoints:create"
        >=> createProtobuf items Url

    /// Delete data points from 1 or more (multiple) time series.
    let delete (items: DataPointsDelete) : IHttpHandler<unit, EmptyResponse> =
        withLogMessage "DataPoints:delete"
        >=> delete items Url

    /// Retrieves the latest data point in multiple time series in the same project.
    let latest (query: DataPointsLatestQuery) : IHttpHandler<unit, IEnumerable<DataPointsItem<DataPoint>>> =
        withLogMessage "DataPoints:latest"
        >=> req {
            let url = Url +/ "latest"
            let! ret = postV10<DataPointsLatestQuery, ItemsWithoutCursor<DataPointsItem<DataPoint>>> query url
            return ret.Items
        }
