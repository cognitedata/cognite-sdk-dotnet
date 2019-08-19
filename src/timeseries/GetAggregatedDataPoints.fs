namespace CogniteSdk.DataPoints

open System.IO
open System.Net.Http

open Com.Cognite.V1.Timeseries.Proto
open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.TimeSeries

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

[<RequireQualifiedAccess>]
type Granularity =
    private
    | CaseDay of int
    | CaseHour of int
    | CaseMinute of int
    | CaseSecond of int
    | CaseNone
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
        | CaseNone -> ""

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
        | _ -> CaseNone

/// Query parameters
//[<RequireQualifiedAccess>]
type AggregateQuery =
    private
    | CaseStart of string
    | CaseEnd of string
    | CaseAggregates of Aggregate seq
    | CaseGranularity of Granularity
    | CaseLimit of int32

    /// Start time as CDF timestamp string
    static member Start start =
        CaseStart start
    /// End time as CDF timestamp string
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
type MultipleAggregateQuery = {
    Id: Identity
    AggregateQuery: AggregateQuery seq
}


[<RequireQualifiedAccess>]
module Aggregated =
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


    let renderQueryOption (param: AggregateQuery) : string * Thoth.Json.Net.JsonValue =
        match param with
        | CaseStart start -> "start", Encode.string start
        | CaseEnd end'  -> "end", Encode.string end'
        | CaseAggregates aggr ->
            let aggregates = aggr |> Seq.map (fun a -> Encode.string (a.ToString ())) |> Array.ofSeq
            "aggregates", Encode.array aggregates
        | CaseGranularity gr -> "granularity", Encode.string (gr.ToString ())
        | CaseLimit limit -> "limit", Encode.int limit

    let renderDataQuery (options: MultipleAggregateQuery seq) (defaultOptions: AggregateQuery seq) =
        Encode.object [
            yield "items", Encode.list [
                for option in options do
                    yield Encode.object [
                        yield! option.AggregateQuery |> Seq.map renderQueryOption
                        match option.Id with
                        | CaseId id -> yield "id", Encode.int64 id
                        | CaseExternalId id -> yield "externalId", Encode.string id
                    ]
            ]

            yield! defaultOptions |> Seq.map renderQueryOption
        ]

    let getAggregatedCore (options: MultipleAggregateQuery seq) (defaultOptions: AggregateQuery seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeProtobuf (DataPointListResponse.Parser.ParseFrom >> decodeToDto)
        let request = renderDataQuery options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> decoder

    let getAggregatedProto (options: MultipleAggregateQuery seq) (defaultOptions: AggregateQuery seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeProtobuf (DataPointListResponse.Parser.ParseFrom)
        let request = renderDataQuery options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregated (id: Identity) (options: AggregateQuery seq) (next: NextHandler<DataPoints seq,'a>) =
        let options' : MultipleAggregateQuery seq = Seq.singleton { Id = id; AggregateQuery = options }
        getAggregatedCore options' Seq.empty fetch next

    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregatedAsync (id: Identity) (options: AggregateQuery seq) =
        let options' : MultipleAggregateQuery seq = Seq.singleton { Id = id; AggregateQuery = options }
        getAggregatedCore options' Seq.empty fetch Async.single

    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>List of aggregated data points in c# protobuf format.</returns>
    let getAggregatedDataPointsProto (id: Identity) (options: AggregateQuery seq) =
        let options' : MultipleAggregateQuery seq = Seq.singleton { Id = id; AggregateQuery = options }
        getAggregatedProto options' Seq.empty fetch Async.single

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregatedMultiple (options: MultipleAggregateQuery seq) (defaultOptions: AggregateQuery seq) (next: NextHandler<DataPoints seq,'a>) =
        getAggregatedCore options defaultOptions fetch next

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of aggregated data points.</returns>
    let getAggregatedMultipleAsync (options: MultipleAggregateQuery seq) (defaultOptions: AggregateQuery seq) =
        getAggregatedCore options defaultOptions fetch Async.single

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of aggregated data points in c# protobuf format.</returns>
    let getAggregatedMultipleProto (options: MultipleAggregateQuery seq) (defaultOptions: AggregateQuery seq) =
        getAggregatedProto options defaultOptions fetch Async.single

namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Com.Cognite.V1.Timeseries.Proto

open Oryx
open CogniteSdk.DataPoints


[<Extension>]
type AggregatedClientExtensions =
    /// <summary>
    /// Retrieves a list of aggregated data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>List of aggregated data points.</returns>
    [<Extension>]
    static member GetAggregatedAsync (this: ClientExtensions.DataPoints, id : Identity, options: AggregateQuery seq, [<Optional>] token: CancellationToken) : Task<DataPointListResponse> =
        async {
            let! ctx = Aggregated.getAggregatedDataPointsProto id options this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)

    /// <summary>
    /// Retrieves a list of aggregated data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of aggregated data points.</returns>
    [<Extension>]
    static member GetAggregatedMultipleAsync (this: ClientExtensions.DataPoints, options: MultipleAggregateQuery seq, defaultOptions: AggregateQuery seq, [<Optional>] token: CancellationToken) : Task<DataPointListResponse> =
        async {
            let! ctx = Aggregated.getAggregatedMultipleProto options defaultOptions this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)

