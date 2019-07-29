namespace Fusion

open System
open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Api
open Fusion.Common
open Fusion.Timeseries
open Com.Cognite.V1.Timeseries.Proto

[<RequireQualifiedAccess>]
module GetAggregatedDataPoints =
    [<Literal>]
    let Url = "/timeseries/data/list"

    type DataPoints = {
        Id: int64
        ExternalId: string option
        DataPoints: AggregateDataPointReadDto seq
    } with
        static member FromProto (data : DataPointListItem) : DataPoints =
            {
                Id = data.Id
                ExternalId = if isNull(data.ExternalId) then None else Some data.ExternalId
                DataPoints =
                    match data.DatapointTypeCase with
                    | (DataPointListItem.DatapointTypeOneofCase.AggregateDatapoints) ->
                        data.AggregateDatapoints.Datapoints |> Seq.map (AggregateDataPointReadDto.FromProto)
                    | _ ->
                        Seq.empty
            }

    type DataResponse = {
        Items: DataPoints seq
    }

    let decodeToDto (data : DataPointListResponse) =
        data.Items |> Seq.map (DataPoints.FromProto)

    type Aggregate =
        private
        | CaseContinuousVariance
        | CaseStepInterpolation
        | CaseDiscreteVariance
        | CaseTotalVariation
        | CaseInterpolation
        | CaseAverage
        | CaseCount
        | CaseMax
        | CaseMin
        | CaseSum

        static member ContinuousVariance = CaseContinuousVariance
        static member StepInterpolation = CaseStepInterpolation
        static member DiscreteVariance = CaseDiscreteVariance
        static member TotalVariation = CaseTotalVariation
        static member Interpolation = CaseInterpolation
        static member Average = CaseAverage
        static member Count = CaseCount
        static member Max = CaseMax
        static member Min = CaseMin
        static member Sum = CaseSum

        override this.ToString () =
            match this with
            | CaseStepInterpolation -> "stepInterpolation"
            | CaseContinuousVariance -> "continuousVariance"
            | CaseDiscreteVariance -> "discreteVariance"
            | CaseInterpolation -> "interpolation"
            | CaseTotalVariation -> "totalVariation"
            | CaseCount -> "count"
            | CaseAverage -> "average"
            | CaseMax -> "max"
            | CaseMin -> "min"
            | CaseSum -> "sum"

        static member FromString str =
            match str with
            | "step" -> Some CaseStepInterpolation
            | "stepInterpolation" -> Some CaseStepInterpolation
            | "cv" -> Some CaseContinuousVariance
            | "continuousVariance" -> Some CaseContinuousVariance
            | "dv" -> Some CaseDiscreteVariance
            | "discreteVariance" -> Some CaseDiscreteVariance
            | "int" -> Some CaseInterpolation
            | "interpolation" -> Some CaseInterpolation
            | "tv" -> Some CaseTotalVariation
            | "totalVariation" -> Some CaseTotalVariation
            | "count" -> Some CaseCount
            | "avg" -> Some CaseAverage
            | "average" -> Some CaseAverage
            | "max" -> Some CaseMax
            | "min" -> Some CaseMin
            | "sum" -> Some CaseSum
            | _ -> None

    type Granularity =
        private
        | CaseDay of int
        | CaseHour of int
        | CaseMinute of int
        | CaseSecond of int
        | None
        /// Granularity by a number of days
        static member Day day =
            CaseDay day
        /// Granularity by a number of hours
        static member Hour hour =
            CaseHour hour
        /// Granularity by a number of minutes
        static member Minute min =
            CaseMinute min
        /// Granularity by a number of seconds
        static member Second sec =
            CaseSecond sec
        override this.ToString () =
            match this with
            | CaseDay day when day = 1 -> "d"
            | CaseDay day -> sprintf "%dd" day
            | CaseHour hour when hour = 1 -> "h"
            | CaseHour hour -> sprintf "%dh" hour
            | CaseMinute min when min = 1 -> "m"
            | CaseMinute min -> sprintf "%dm" min
            | CaseSecond sec when sec = 1 -> "s"
            | CaseSecond sec -> sprintf "%dm" sec
            | None -> ""

        static member FromString str =
            match str with
            | ParseRegex "(\d{1,3})d" [ ParseInteger day ] -> CaseDay day
            | ParseRegex "^d$" [] -> CaseDay 1
            | ParseRegex "(\d{1,3})h" [ ParseInteger hour ] -> CaseHour hour
            | ParseRegex "^h$" [] -> CaseHour 1
            | ParseRegex "(\d{1,3})m" [ ParseInteger min ] -> CaseMinute min
            | ParseRegex "^m$" [] -> CaseMinute 1
            | ParseRegex "(\d{1,3})s" [ ParseInteger sec ] -> CaseSecond sec
            | ParseRegex "^s$" [] -> CaseSecond 1
            | _ -> None

    /// Query parameters
    type QueryOption =
        private
        | CaseStart of string
        | CaseEnd of string
        | CaseAggregates of Aggregate seq
        | CaseGranularity of Granularity
        | CaseLimit of int32
        /// Start time as cdf timestamp string
        static member Start start =
            CaseStart start
        /// End time as cdf timestamp string
        static member End stop =
            CaseEnd stop
        /// List of aggregates
        static member Aggregates aggregates =
            CaseAggregates aggregates
        /// Granularity for aggregates
        static member Granularity granularity =
            CaseGranularity granularity
        /// Max number of results to return
        static member Limit limit =
            CaseLimit limit

     [<CLIMutable>]
    type Option = {
        Id: Identity
        QueryOptions: QueryOption seq
    }

    let renderQueryOption (param: QueryOption) : string * Thoth.Json.Net.JsonValue =
        match param with
        | CaseStart start -> "start", Encode.string start
        | CaseEnd end'  -> "end", Encode.string end'
        | CaseAggregates aggr ->
            let aggregates = aggr |> Seq.map (fun a -> Encode.string (a.ToString ())) |> Array.ofSeq
            "aggregates", Encode.array aggregates
        | CaseGranularity gr -> "granularity", Encode.string (gr.ToString ())
        | CaseLimit limit -> "limit", Encode.int limit

    let renderDataQuery (options: Option seq) (defaultOptions: QueryOption seq) =
        Encode.object [
            yield "items", Encode.list [
                for option in options do
                    yield Encode.object [
                        yield! option.QueryOptions |> Seq.map renderQueryOption
                        match option.Id with
                        | CaseId id -> yield "id", Encode.int64 id
                        | CaseExternalId id -> yield "externalId", Encode.string id
                    ]
            ]

            yield! defaultOptions |> Seq.map renderQueryOption
        ]

    let getAggregatedDataPoints (options: Option seq) (defaultOptions: QueryOption seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeProtobuf (DataPointListResponse.Parser.ParseFrom >> decodeToDto)
        let request = renderDataQuery options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> decoder

    let getAggregatedDataPointsProto (options: Option seq) (defaultOptions: QueryOption seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeProtobuf (DataPointListResponse.Parser.ParseFrom)
        let request = renderDataQuery options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetAggregatedDataPointsApi =
    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregatedDataPoints (id: Identity) (options: GetAggregatedDataPoints.QueryOption seq) (next: NextHandler<GetAggregatedDataPoints.DataPoints seq,'a>) =
        let options' : GetAggregatedDataPoints.Option seq = Seq.singleton { Id = id; QueryOptions = options }
        GetAggregatedDataPoints.getAggregatedDataPoints options' Seq.empty fetch next

    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregatedDataPointsAsync (id: Identity) (options: GetAggregatedDataPoints.QueryOption seq) =
        let options' : GetAggregatedDataPoints.Option seq = Seq.singleton { Id = id; QueryOptions = options }
        GetAggregatedDataPoints.getAggregatedDataPoints options' Seq.empty fetch Async.single

    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>List of aggregated data points in c# protobuf format.</returns>
    let getAggregatedDataPointsProto (id: Identity) (options: GetAggregatedDataPoints.QueryOption seq) =
        let options' : GetAggregatedDataPoints.Option seq = Seq.singleton { Id = id; QueryOptions = options }
        GetAggregatedDataPoints.getAggregatedDataPointsProto options' Seq.empty fetch Async.single

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregatedDataPointsMultiple (options: GetAggregatedDataPoints.Option seq) (defaultOptions: GetAggregatedDataPoints.QueryOption seq) (next: NextHandler<GetAggregatedDataPoints.DataPoints seq,'a>) =
        GetAggregatedDataPoints.getAggregatedDataPoints options defaultOptions fetch next

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregatedDataPointsMultipleAsync (options: GetAggregatedDataPoints.Option seq) (defaultOptions: GetAggregatedDataPoints.QueryOption seq) =
        GetAggregatedDataPoints.getAggregatedDataPoints options defaultOptions fetch Async.single

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of aggregated data points in c# protobuf format.</returns>
    let getAggregatedDataPointsMultipleProto (options: GetAggregatedDataPoints.Option seq) (defaultOptions: GetAggregatedDataPoints.QueryOption seq) =
        GetAggregatedDataPoints.getAggregatedDataPointsProto options defaultOptions fetch Async.single

[<Extension>]
type GetAggregatedDataPointsExtensions =
    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>List of aggregated data points.</returns>
    static member GetAggregatedDataPointsAsync (this: Client, id : Identity, options: GetAggregatedDataPoints.QueryOption seq) : Task<DataPointListResponse> =
        task {
            let! ctx = getAggregatedDataPointsProto id options this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
                let err = error2Exception error
                return raise err
        }

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of aggregated data points.</returns>
    [<Extension>]
    static member GetAggregatedDataPointsMultipleAsync (this: Client, options: GetAggregatedDataPoints.Option seq, defaultOptions: GetAggregatedDataPoints.QueryOption seq) : Task<DataPointListResponse> =
        task {
            let! ctx = getAggregatedDataPointsMultipleProto options defaultOptions this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
                let err = error2Exception error
                return raise err
        }

