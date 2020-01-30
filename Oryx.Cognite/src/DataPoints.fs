// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Collections.Generic
open System.Net.Http

open Com.Cognite.V1.Timeseries.Proto
open Oryx
open Oryx.Cognite

open CogniteSdk
open CogniteSdk.DataPoints


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
    let create (items: DataPointInsertionRequest) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        createProtobuf items Url

    /// Delete data points from 1 or more (multiple) time series.
    let delete (items: DataPointsDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete items Url

    /// Retrieves the latest data point in multiple time series in the same project.
    let latest (query: DataPointsLatestQueryDto) : HttpHandler<HttpResponseMessage, IEnumerable<DataPointsReadDto<DataPointAggregateDto>>, 'a> =
        req {
            let url = Url +/ "latest"
            let! ret = post<DataPointsLatestQueryDto, ItemsWithoutCursor<DataPointsReadDto<DataPointAggregateDto>>, 'a> query url
            return ret.Items
        }