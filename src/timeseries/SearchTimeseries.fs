namespace Fusion

open System.IO
open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Api
open Fusion.Timeseries
open Fusion.Common

[<RequireQualifiedAccess>]
module SearchTimeseries =
    [<Literal>]
    let Url = "/timeseries/search"

    type Option =
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

        static member Encode (options: Option seq) =
            Encode.object [
                for option in options do
                    match option with
                    | CaseName name -> yield "name", Encode.string name
                    | CaseDescription desc -> yield "description", Encode.string desc
                    | CaseQuery query -> yield "query", Encode.string query
            ]

    type Filter =
        private
        | CaseName of string
        | CaseUnit of string
        | CaseIsString of bool
        | CaseIsStep of bool
        | CaseMetaData of Map<string, string>
        | CaseAssetIds of int64 seq
        | CaseExternalIdPrefix of string
        | CaseCreatedTime of int64
        | CaseLastUpdatedTime of int64

        /// Name of asset. Often referred to as tag.
        static member Name name = CaseName name
        static member Unit unit = CaseUnit unit
        static member IsString isString = CaseIsString isString
        static member IsStep isStep = CaseIsStep isStep
        static member MetaData (metaData : IDictionary<string, string>) =
            metaData |> Seq.map (|KeyValue|) |> Map.ofSeq |> CaseMetaData
        static member AssetIds ids = CaseAssetIds ids
        static member ExternalIdPrefix prefix = CaseExternalIdPrefix prefix
        static member CreatedTime time = CaseCreatedTime time
        static member LastUpdatedTime time = CaseLastUpdatedTime time

        static member Encode (filters: Filter seq) =
            Encode.object [
                for filter in filters do
                    match filter with
                    | CaseName name -> yield "name", Encode.string name
                    | CaseUnit unit -> yield "unit", Encode.string unit
                    | CaseIsStep isStep -> yield "isStep", Encode.bool isStep
                    | CaseIsString isString -> yield "isString", Encode.bool isString
                    | CaseMetaData md -> yield "metaData", Encode.propertyBag md
                    | CaseAssetIds ids -> yield "assetIds", Encode.int53seq ids
                    | CaseExternalIdPrefix prefix -> yield "externalIdPrefix", Encode.string prefix
                    | CaseCreatedTime time -> yield "createdTime", Encode.int53 time
                    | CaseLastUpdatedTime time -> yield "lastUpdatedTime", Encode.int53 time
            ]

    type Timeseries = {
        Items: TimeseriesReadDto seq } with

        static member Decoder : Decoder<Timeseries> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeseriesReadDto.Decoder |> Decode.map seq)
            })

    let encodeRequest limit options filters =
        Encode.object [
            if limit > 0 then
                yield "limit", Encode.int limit
            if not (Seq.isEmpty filters) then
                yield "filter", Filter.Encode filters
            if not (Seq.isEmpty options) then
                yield "search", Option.Encode options
        ]

    let searchTimeseries (limit: int) (options: Option seq) (filters: Filter seq)(fetch: HttpHandler<HttpResponseMessage,Stream, 'a>) =
        let decoder = decodeResponse GetTimeseries.TimeseriesResponse.Decoder (fun assets -> assets.Items)
        let body = encodeRequest limit options filters

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module SearchTimeseriesApi =

    /// **Description**
    ///
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    ///
    /// **Parameters**
    ///
    ///   * `limit` - Limits the maximum number of results to be returned by single request. In case there are more
    ///   results to the request 'nextCursor' attribute will be provided as part of response. Request may contain less
    ///   results than request limit.
    ///   * `options` - Search options.
    ///   * `filters` - Search filters.
    ///
    /// <returns>Context of assets.</returns>
    let searchTimeseries (limit: int) (options: SearchTimeseries.Option seq) (filters: SearchTimeseries.Filter seq) (next: NextHandler<TimeseriesReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        SearchTimeseries.searchTimeseries limit options filters fetch next

    /// **Description**
    ///
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    ///
    /// **Parameters**
    ///
    ///   * `limit` - Limits the maximum number of results to be returned by single request. In case there are more
    ///   results to the request 'nextCursor' attribute will be provided as part of response. Request may contain less
    ///   results than request limit.
    ///   * `options` - Search options.
    ///   * `filters` - Search filters.
    ///
    /// <returns>Context of assets.</returns>
    let searchTimeseriesAsync (limit: int) (options: SearchTimeseries.Option seq) (filters: SearchTimeseries.Filter seq): HttpContext -> Async<Context<TimeseriesReadDto seq>> =
        SearchTimeseries.searchTimeseries limit options filters fetch Async.single

[<Extension>]
type SearchTimeseriesExtensions =
    /// <summary>
    /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    ///   * `limit` - Limits the maximum number of results to be returned by single request. In case there are more
    ///   results to the request 'nextCursor' attribute will be provided as part of response. Request may contain less
    ///   results than request limit.
    ///   * `options` - Search options.
    ///   * `filters` - Search filters.
    ///
    /// <returns>Assets.</returns>
    [<Extension>]
    static member SearchTimeseriesAsync (this: Client, limit : int, options: SearchTimeseries.Option seq, filters: SearchTimeseries.Filter seq) : Task<_ seq> =
        task {
            let! ctx = searchTimeseriesAsync limit options filters this.Ctx
            match ctx.Result with
            | Ok tss ->
                return tss |> Seq.map (fun ts -> ts.ToPoco ())
            | Error error ->
                let! err = error2Exception error
                return raise err
        }
