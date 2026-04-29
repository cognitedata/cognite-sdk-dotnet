// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System.IO.Compression

open Com.Cognite.V1.Timeseries.Proto
open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk

/// Time series data point handlers that opt in to the beta API
[<RequireQualifiedAccess>]
module DataPoints =
    [<Literal>]
    let Url = "/timeseries/data"

    /// Retrieves data points using the beta API
    let list (query: DataPointsQuery) (source: HttpHandler<unit>) : HttpHandler<DataPointListResponse> =
        source
        |> withLogMessage "Beta:DataPoints:list"
        |> withBetaHeader
        |> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Retrieves aggregate data points using the beta API
    let listAggregates (query: DataPointsQuery) (source: HttpHandler<unit>) : HttpHandler<DataPointListResponse> =
        source
        |> withLogMessage "Beta:DataPoints:listAggregates"
        |> withBetaHeader
        |> listProtobuf query Url DataPointListResponse.Parser.ParseFrom

    /// Insert data points using the beta API
    let create (items: DataPointInsertionRequest) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Beta:DataPoints:create"
        |> withBetaHeader
        |> createProtobuf items Url

    /// Insert data points with gzip compression using the beta API
    let createWithGzip
        (items: DataPointInsertionRequest)
        (compression: CompressionLevel)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Beta:DataPoints:create"
        |> withBetaHeader
        |> createGzipProtobuf items compression Url
