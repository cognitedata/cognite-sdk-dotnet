namespace Cognite.Sdk

open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common
open Cognite.Sdk.Timeseries
open System

[<RequireQualifiedAccess>]
module GetDataPoints =
    [<Literal>]
    let Url = "/timeseries/data/list"

    type DataPointsPoco = {
        Id : int64
        ExternalId : string
        IsString : bool
        DataPoints : DataPointDto seq
    }

    type DataPoints = {
        Id: int64
        ExternalId: string option
        IsString: bool
        DataPoints: DataPointDto seq
    } with
        static member Decoder : Decoder<DataPoints> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "exteralId" Decode.string
                    IsString = get.Required.Field "isString" Decode.bool
                    DataPoints = get.Required.Field "datapoints" (Decode.list DataPointDto.Decoder)
                })
        member this.ToPoco () : DataPointsPoco =
            {
                Id = this.Id
                ExternalId = if this.ExternalId.IsSome then this.ExternalId.Value else null
                IsString = this.IsString
                DataPoints = this.DataPoints
            }

    type DataResponse = {
        Items: DataPoints seq
    } with
        static member Decoder : Decoder<DataResponse> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list DataPoints.Decoder)
                })

    /// Query parameters
    type Option =
        private
        | CaseStart of string
        | CaseEnd of string
        | CaseLimit of int32
        | CaseIncludeOutsidePoints of bool

        static member Start start =
            CaseStart start
        static member End stop =
            CaseEnd stop
        static member Limit limit =
            CaseLimit limit
        static member IncludeOutsidePoints iop =
            CaseIncludeOutsidePoints iop

    let renderQuery (param: Option) : string*Thoth.Json.Net.JsonValue =
        match param with
        | CaseStart start -> "start", Encode.string start
        | CaseEnd end'  -> "end", Encode.string end'
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseIncludeOutsidePoints iop -> "includeOutsidePoints", Encode.bool iop

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

    let getDataPoints (defaultOptions: Option seq) (options: (int64*(Option seq)) seq) (fetch: HttpHandler<HttpResponseMessage, string, 'a>) =
        let decoder = decodeResponse DataResponse.Decoder (fun res -> res.Items)
        let request = renderDataQuery defaultOptions options
        let body = Encode.stringify request

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setBody body
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetDataPointsApi =

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
    let getDataPoints (defaultArgs: GetDataPoints.Option seq) (args: (int64*(GetDataPoints.Option seq)) seq) (next: NextHandler<GetDataPoints.DataPoints seq,'a>) =
        GetDataPoints.getDataPoints defaultArgs args fetch next

    let getDataPointsAsync (defaultQueryParams: GetDataPoints.Option seq) (queryParams: (int64*(GetDataPoints.Option seq)) seq) =
        GetDataPoints.getDataPoints defaultQueryParams queryParams fetch Async.single

[<Extension>]
type GetDataPointsExtensions =
    // <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="defaultQuery">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <param name="query">Parameters describing a query for multiple datapoints.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member GetDataPointsAsync (this: Client) (defaultOptions: GetDataPoints.Option seq) (options: ValueTuple<int64, GetDataPoints.Option seq> seq) : Task<seq<GetDataPoints.DataPointsPoco>> =

        let options' = options |> Seq.map (fun struct (key, value) -> key, value)

        task {
            let! ctx = getDataPointsAsync defaultOptions options' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun points -> points.ToPoco ())
            | Error error ->
               return raise (Error.error2Exception error)
        }

