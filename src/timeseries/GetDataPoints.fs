namespace Cognite.Sdk

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common
open Cognite.Sdk.Timeseries
open System.Collections.Generic


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
    type QueryOption =
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

    type DefaultOption = QueryOption

    [<CLIMutable>]
    type Option = {
        Id: int64
        QueryOptions: QueryOption seq
    }

    let renderQueryOption (option: QueryOption) : string*Thoth.Json.Net.JsonValue =

        let a : ICollection<int> = ResizeArray () :> _

        match option with
        | CaseStart start -> "start", Encode.string start
        | CaseEnd end'  -> "end", Encode.string end'
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseIncludeOutsidePoints iop -> "includeOutsidePoints", Encode.bool iop

    let renderRequest (options: Option seq) (defaultOptions: QueryOption seq) =
        Encode.object [
            yield "items", Encode.list [
                for option in options do
                    yield Encode.object [
                        yield "id", Encode.int64 option.Id
                        yield! option.QueryOptions |> Seq.map renderQueryOption
                    ]
            ]
            yield! defaultOptions |> Seq.map renderQueryOption
        ]
    let getDataPoints (options: Option seq) (defaultOptions: QueryOption seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse DataResponse.Decoder (fun res -> res.Items)
        let request = renderRequest options defaultOptions
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
    /// Retrieves a list of data points from single time series in the same project
    ///
    /// **Parameters**
    ///   * `id` - Id of timeseries to retrieve datapoints from.
    ///   * `options` - Sequence of `GetDataPoints.Option` to be used as default options.
    ///   * `next` - Next handler to invoke after this handler.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Context<HttpResponseMessage> -> Async<Context<'a>>`
    ///
    let getDataPoints (id: int64) (options: GetDataPoints.QueryOption seq) (next: NextHandler<GetDataPoints.DataPoints seq,'a>) =
        let options' : GetDataPoints.Option seq = Seq.singleton { Id = id; QueryOptions = options }
        GetDataPoints.getDataPoints options' Seq.empty fetch next

    /// **Description**
    ///
    /// Retrieves a list of data points from single time series in the same project
    ///
    /// **Parameters**
    ///   * `id` - Id of timeseries to retrieve datapoints from.
    ///   * `options` - Sequence of `GetDataPoints.Option` to be used as default options.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Context<seq<GetDataPoints.DataPoints>>>`
    ///
    let getDataPointsAsync (id: int64) (options: GetDataPoints.QueryOption seq) =
        let options' : GetDataPoints.Option seq = Seq.singleton { Id = id; QueryOptions = options }
        GetDataPoints.getDataPoints options' Seq.empty fetch Async.single

    /// **Description**
    ///
    /// Retrieves a list of data points from multiple time series in the same project
    ///
    /// **Parameters**
    ///   * `options` - Sequence of `GetDataPoints.Options` describing the query for each timeseries.
    ///   * `defaultOptions` - Sequence of `GetDataPoints.Option` to be used as default options.
    ///   * `next` - Next handler to invoke after this handler.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Context<HttpResponseMessage> -> Async<Context<'a>>`
    ///
    let getDataPointsMultiple (options: GetDataPoints.Option seq) (defaultOptions: GetDataPoints.QueryOption seq) (next: NextHandler<GetDataPoints.DataPoints seq,'a>) =
        GetDataPoints.getDataPoints options defaultOptions fetch next

    /// **Description**
    ///
    /// Retrieves a list of data points from multiple time series in the same project
    ///
    /// **Parameters**
    ///   * `options` - Sequence of `GetDataPoints.Options` describing the query for each timeseries.
    ///   * `defaultOptions` - Sequence of `GetDataPoints.Option` to be used as default options.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Context<HttpResponseMessage> -> Async<Context<'a>>`
    ///
    let getDataPointsMultipleAsync (options: GetDataPoints.Option seq) (defaultOptions: GetDataPoints.QueryOption seq)=
        GetDataPoints.getDataPoints options defaultOptions fetch Async.single

[<Extension>]
type GetDataPointsExtensions =

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>Http status code.</returns>
    static member GetDataPointsAsync (this: Client, id : int64, options: GetDataPoints.QueryOption seq) : Task<seq<GetDataPoints.DataPointsPoco>> =
        task {
            let! ctx = getDataPointsAsync id options this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun points -> points.ToPoco ())
            | Error error ->
                let! err = error2Exception error
                return raise err
        }

    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member GetDataPointsMultipleAsync (this: Client, options: GetDataPoints.Option seq, defaultOptions: GetDataPoints.QueryOption seq) : Task<seq<GetDataPoints.DataPointsPoco>> =
        task {
            let! ctx = getDataPointsMultipleAsync options defaultOptions this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun points -> points.ToPoco ())
            | Error error ->
                let! err = error2Exception error
                return raise err
        }

