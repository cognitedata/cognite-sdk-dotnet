// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System.Collections.Generic
open System.IO.Compression

open Com.Cognite.V1.Timeseries.Proto.Beta
open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various time series data points HTTP handlers

[<RequireQualifiedAccess>]
module BetaDataPoints =
    [<Literal>]
    let Url = "/timeseries/data"

    /// Retrieves a list of numeric data points from multiple time series in a project.
    let list (query: DataPointsQuery) (source: HttpHandler<unit>) : HttpHandler<DataPointListResponse> =
        source
        |> withLogMessage "DataPoints:list"
        |> withBetaHeader
        |> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Retrieves a list of aggregate data points from multiple time series in a project.
    let listAggregates (query: DataPointsQuery) (source: HttpHandler<unit>) : HttpHandler<DataPointListResponse> =
        source
        |> withLogMessage "DataPoints:listAggregates"
        |> withBetaHeader
        |> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Create one or more new timeseries. Returns a list of created time series.
    let create (items: DataPointInsertionRequest) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "DataPoints:create"
        |> withBetaHeader
        |> createProtobuf items Url

    let createWithGzip
        (items: DataPointInsertionRequest)
        (compression: CompressionLevel)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "DataPoints:create"
        |> withBetaHeader
        |> createGzipProtobuf items compression Url

    /// Delete data points from 1 or more (multiple) time series.
    let delete (items: DataPointsDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "DataPoints:delete"
        |> withBetaHeader
        |> delete items Url

    /// Retrieves the latest data point in multiple time series in the same project.
    let latest
        (query: DataPointsLatestQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<DataPointsItem<DataPoint>>> =
        http {
            let url = Url +/ "latest"

            let! ret =
                source
                |> withLogMessage "DataPoints:latest"
                |> withBetaHeader
                |> postV10<DataPointsLatestQuery, ItemsWithoutCursor<DataPointsItem<DataPoint>>> query url

            return ret.Items
        }
