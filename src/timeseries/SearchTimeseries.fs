// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries

open System.Collections.Generic
open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net
open CogniteSdk
open CogniteSdk.TimeSeries



type TimeSeriesSearch =
    private
    | CaseName of string
    | CaseDescription of string
    | CaseQuery of string

    /// Prefix and fuzzy search on name.
    static member Name name = CaseName name
    /// Prefix and fuzzy search on description.
    static member Description description = CaseDescription description
    /// Search on name and description using wildcard search on each of the
    /// words (separated by spaces). Retrieves results where at least one
    /// word must match. Example: 'some other'
    static member Query query = CaseQuery query

    static member Encode (options: TimeSeriesSearch seq) =
        Encode.object [
            for option in options do
                match option with
                | CaseName name -> yield "name", Encode.string name
                | CaseDescription desc -> yield "description", Encode.string desc
                | CaseQuery query -> yield "query", Encode.string query
        ]

type TimeSeriesFilter =
    private
    | CaseName of string
    | CaseUnit of string
    | CaseIsString of bool
    | CaseIsStep of bool
    | CaseMetaData of Map<string, string>
    | CaseAssetIds of int64 seq
    | CaseRootAssetIds of int64 seq
    | CaseExternalIdPrefix of string
    | CaseCreatedTime of int64
    | CaseLastUpdatedTime of int64

    /// Name of timeseries
    static member Name name = CaseName name
    /// Unit of data in timeseries
    static member Unit unit = CaseUnit unit
    /// True if datatype of timeseries is string
    static member IsString isString = CaseIsString isString
    /// True if timeseries does not interpolate between datapoints
    static member IsStep isStep = CaseIsStep isStep
    /// Metadata of timeseries as string key -> string value
    static member MetaData (metaData : IDictionary<string, string>) =
        metaData |> Seq.map (|KeyValue|) |> Map.ofSeq |> CaseMetaData
    /// Filter out timeseries without assetId in list
    static member AssetIds ids = CaseAssetIds ids
    /// Prefix on externalId of timeseries
    static member ExternalIdPrefix prefix = CaseExternalIdPrefix prefix
    /// Filter out assets without this exact createdTime
    static member CreatedTime time = CaseCreatedTime time
    /// Filter out assets without this exact updatedTime
    static member LastUpdatedTime time = CaseLastUpdatedTime time

    static member Encode (filters: TimeSeriesFilter seq) =
        Encode.object [
            for filter in filters do
                match filter with
                | CaseName name -> yield "name", Encode.string name
                | CaseUnit unit -> yield "unit", Encode.string unit
                | CaseIsStep isStep -> yield "isStep", Encode.bool isStep
                | CaseIsString isString -> yield "isString", Encode.bool isString
                | CaseMetaData md -> yield "metaData", Encode.propertyBag md
                | CaseAssetIds ids -> yield "assetIds", Encode.int53seq ids
                | CaseRootAssetIds ids -> yield "rootAssetIds", Encode.int53seq ids
                | CaseExternalIdPrefix prefix -> yield "externalIdPrefix", Encode.string prefix
                | CaseCreatedTime time -> yield "createdTime", Encode.int53 time
                | CaseLastUpdatedTime time -> yield "lastUpdatedTime", Encode.int53 time
        ]


[<RequireQualifiedAccess>]
module Search =
    [<Literal>]
    let Url = "/timeseries/search"


    type Timeseries = {
        Items: TimeSeriesReadDto seq } with

        static member Decoder : Decoder<Timeseries> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeSeriesReadDto.Decoder |> Decode.map seq)
            })

    let encodeRequest limit options filters =
        Encode.object [
            if limit > 0 then
                yield "limit", Encode.int limit
            if not (Seq.isEmpty filters) then
                yield "filter", TimeSeriesFilter.Encode filters
            if not (Seq.isEmpty options) then
                yield "search", TimeSeriesSearch.Encode options
        ]

    let searchCore (limit: int) (options: TimeSeriesSearch seq) (filters: TimeSeriesFilter seq)(fetch: HttpHandler<HttpResponseMessage,Stream, 'a>) =
        let decoder = Encode.decodeResponse Items.TimeSeriesItemsDto.Decoder (fun assets -> assets.Items)
        let body = encodeRequest limit options filters

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue body)
        >=> setResource Url
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Timeseries matching query.</returns>>
    let search (limit: int) (options: TimeSeriesSearch seq) (filters: TimeSeriesFilter seq) (next: NextFunc<TimeSeriesReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        searchCore limit options filters fetch next

    /// <summary>
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    /// <returns>Timeseries matching query.</returns>
    let searchAsync (limit: int) (options: TimeSeriesSearch seq) (filters: TimeSeriesFilter seq): HttpContext -> Async<Context<TimeSeriesReadDto seq>> =
        searchCore limit options filters fetch Async.single

[<Extension>]
type SearchTimeSeriesClientExtensions =
    /// <summary>
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    /// <returns>Timeseries matching query.</returns>
    [<Extension>]
    static member SearchAsync (this: ClientExtension, limit : int, options: TimeSeriesSearch seq, filters: TimeSeriesFilter seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        async {
            let! ctx = Search.searchAsync limit options filters this.Ctx
            match ctx.Result with
            | Ok tss ->
                return tss |> Seq.map (fun ts -> ts.ToTimeSeriesEntity ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
