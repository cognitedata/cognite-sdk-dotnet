namespace Cognite.Sdk

open System
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common
open Cognite.Sdk.Timeseries

[<RequireQualifiedAccess>]
module GetAggregatedData =
    [<Literal>]
    let Url = "/timeseries/data/list"

    [<CLIMutable>]
    type DataPointsPoco = {
        Id: int64
        ExternalId: string
        DataPoints: AggregateDataPointReadPoco seq
    }

    type DataPoints = {
        Id: int64
        ExternalId: string option
        DataPoints: AggregateDataPointReadDto seq
    } with
        static member Decoder : Decoder<DataPoints> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "exteralId" Decode.string
                    DataPoints = get.Required.Field "datapoints" (Decode.list AggregateDataPointReadDto.Decoder)
                })
        member this.ToPoco() : DataPointsPoco =
            {
                Id = this.Id
                ExternalId = if this.ExternalId.IsSome then this.ExternalId.Value else null
                DataPoints = this.DataPoints |> Seq.map (fun pt -> pt.ToPoco ())
            }

    type DataResponse = {
        Items: DataPoints seq
    } with
        static member Decoder : Decoder<DataResponse> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list DataPoints.Decoder)
                })

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
            | CaseStepInterpolation -> "step"
            | CaseContinuousVariance -> "cv"
            | CaseDiscreteVariance -> "dv"
            | CaseInterpolation -> "int"
            | CaseTotalVariation -> "tv"
            | CaseCount -> "count"
            | CaseAverage -> "avg"
            | CaseMax -> "max"
            | CaseMin -> "min"
            | CaseSum -> "sum"

    type Granularity =
        private
        | CaseDay of int
        | CaseHour of int
        | CaseMinute of int
        | CaseSecond of int

        static member Day day =
            CaseDay day
        static member Hour hour =
            CaseHour hour
        static member Minute min =
            CaseMinute min
        static member Second sec =
            CaseSecond sec
        override this.ToString () =
            match this with
            | CaseDay day when day = 1 -> "d"
            | CaseDay day -> sprintf "d%d" day
            | CaseHour hour when hour = 1 -> "h"
            | CaseHour hour -> sprintf "h%d" hour
            | CaseMinute min when min = 1 -> "m"
            | CaseMinute min -> sprintf "m%d" min
            | CaseSecond sec when sec = 1 -> "s"
            | CaseSecond sec -> sprintf "s%d" sec

    /// Query parameters
    type Option =
        private
        | CaseStart of string
        | CaseEnd of string
        | CaseAggregates of Aggregate seq
        | CaseGranularity of Granularity
        | CaseLimit of int32

        static member Start start =
            CaseStart start
        static member End stop =
            CaseEnd stop
        static member Aggregates aggregates =
            CaseAggregates aggregates
        static member Granularity granularity =
            CaseGranularity granularity
        static member Limit limit =
            CaseLimit limit

    let renderQuery (param: Option) : string*Thoth.Json.Net.JsonValue =
        match param with
        | CaseStart start -> "start", Encode.string start
        | CaseEnd end'  -> "end", Encode.string end'
        | CaseAggregates aggr ->
            let aggregates = aggr |> Seq.map (fun a -> Encode.string (a.ToString ())) |> Array.ofSeq
            "aggregates", Encode.array aggregates
        | CaseGranularity gr -> "granularity", Encode.string (gr.ToString ())
        | CaseLimit limit -> "limit", Encode.int limit

    let renderDataQuery (defaultQuery: Option seq) (args: (int64*(Option seq)) seq) =
        Encode.object [
            yield "items", Encode.list [
                for (id, arg) in args do
                    yield Encode.object [
                        for param in arg do
                            yield renderQuery param
                        yield "id", Encode.int64 id
                    ]
            ]

            for param in defaultQuery do
                yield renderQuery param
        ]

    let getTimeseriesData (defaultOptions: Option seq) (options: (int64*(Option seq)) seq) (fetch: HttpHandler<HttpResponseMessage, string, 'a>) =
        let url = Url + "/data/list"
        let decoder = decodeResponse DataResponse.Decoder (fun res -> res.Items)
        let request = renderDataQuery defaultOptions options
        let body = Encode.stringify request

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setBody body
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetAggregatedDataApi =

    /// **Description**
    ///
    /// Retrieves a list of data points from multiple time series in the same project
    ///
    /// **Parameters**
    ///   * `name` - parameter of type `string`
    ///   * `query` - parameter of type `QueryParams seq`
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let getTimeseriesData (defaultOptions: GetAggregatedData.Option seq) (options: (int64*(GetAggregatedData.Option seq)) seq) (next: NextHandler<GetAggregatedData.DataPoints seq,'a>) =
        GetAggregatedData.getTimeseriesData defaultOptions options fetch next

    let getTimeseriesDataAsync (defaultOptions: GetAggregatedData.Option seq) (options: (int64*(GetAggregatedData.Option seq)) seq) =
        GetAggregatedData.getTimeseriesData defaultOptions options fetch Async.single

[<Extension>]
type GetAggregatedDataExtensions =
    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="defaultQuery">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <param name="query">Parameters describing a query for multiple datapoints.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member GetAggregatedDataAsync (this: Client) (defaultOptions: GetAggregatedData.Option seq) (options: ValueTuple<int64, GetAggregatedData.Option seq> seq) : Task<GetAggregatedData.DataPointsPoco seq> =
        task {
            let options' = options |> Seq.map (fun struct (id, options) -> id, options)
            let! ctx = getTimeseriesDataAsync defaultOptions options' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun points -> points.ToPoco ())
            | Error error ->
               return raise (Error.error2Exception error)
        }

