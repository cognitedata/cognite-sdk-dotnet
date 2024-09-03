// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open System.Collections.Generic
open System.IO.Compression

open Com.Cognite.V1.Timeseries.Proto
open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha


/// Various time series data points HTTP handlers

[<RequireQualifiedAccess>]
module DataPoints =
    [<Literal>]
    let Url = "/timeseries/data"

    /// Retrieves a list of numeric data points from multiple time series in a project.
    let list (query: DataPointsQuery) (source: HttpHandler<unit>) : HttpHandler<DataPointListResponse> =
        source
        |> withLogMessage "DataPoints:list"
        |> withAlphaHeader
        |> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Retrieves a list of aggregate data points from multiple time series in a project.
    let listAggregates (query: DataPointsQuery) (source: HttpHandler<unit>) : HttpHandler<DataPointListResponse> =
        source
        |> withLogMessage "DataPoints:listAggregates"
        |> withAlphaHeader
        |> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Create one or more new times eries. Returns a list of created time series.
    let create (items: DataPointInsertionRequest) (source: HttpHandler<unit>) : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "DataPoints:create"
        |> withAlphaHeader
        |> createProtobuf items Url

    let createWithGzip
        (items: DataPointInsertionRequest)
        (compression: CompressionLevel)
        (source: HttpHandler<unit>)
        : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "DataPoints:create"
        |> withAlphaHeader
        |> createGzipProtobuf items compression Url

    /// Delete data points from 1 or more (multiple) time series.
    let delete (items: DataPointsDelete) (source: HttpHandler<unit>) : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "DataPoints:delete"
        |> withAlphaHeader
        |> delete items Url

    /// Retrieves the latest data point in multiple time series in the same project.
    let latest
        (query: DataPointsLatestQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<DataPointsItem<CogniteSdk.DataPoint>>> =
        http {
            let url = Url +/ "latest"

            let! ret =
                source
                |> withLogMessage "DataPoints:latest"
                |> withAlphaHeader
                |> postV10<DataPointsLatestQuery, CogniteSdk.ItemsWithoutCursor<DataPointsItem<CogniteSdk.DataPoint>>>
                    query
                    url

            return ret.Items
        }
