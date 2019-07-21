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
module GetDataPoints =
    [<Literal>]
    let Url = "/timeseries/data/list"

    type DataPoints = {
        Id: int64
        ExternalId: string option
        IsString: bool
        DataPoints: DataPointSeq
    } with
        static member FromProto (data : DataPointListItem) : DataPoints =
            {
                Id = data.Id
                ExternalId = if isNull(data.ExternalId) then None else Some data.ExternalId
                IsString = data.DatapointTypeCase = DataPointListItem.DatapointTypeOneofCase.StringDatapoints
                DataPoints =
                    match data.DatapointTypeCase with
                    | (DataPointListItem.DatapointTypeOneofCase.StringDatapoints) ->
                        data.StringDatapoints.Datapoints |> Seq.map (StringDataPointDto.FromProto) |> String
                    | (DataPointListItem.DatapointTypeOneofCase.NumericDatapoints) ->
                        data.NumericDatapoints.Datapoints |> Seq.map (NumericDataPointDto.FromProto) |> Numeric
                    | _ ->
                        Seq.empty |> Numeric
            }

    let decodeToDto (data : DataPointListResponse) : seq<DataPoints> =
        data.Items |> Seq.map (DataPoints.FromProto)

    type DataResponse = {
        Items: DataPoints seq
    }

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
        Id: Identity
        QueryOptions: QueryOption seq
    }

    let renderQueryOption (option: QueryOption) : string*Thoth.Json.Net.JsonValue =
        match option with
        | CaseStart start -> "start", Encode.string start
        | CaseEnd end'  -> "end", Encode.string end'
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseIncludeOutsidePoints iop -> "includeOutsidePoints", Encode.bool iop

    let renderRequest (options: Option seq) (defaultOptions: QueryOption seq) =
        Encode.object [
            yield "items", Encode.list (
                options
                |> Seq.map (fun (option: Option) ->
                    Encode.object [
                        yield option.Id.Render
                        yield! option.QueryOptions
                        |> Seq.map renderQueryOption
                        |> List.ofSeq
                    ])
                |> List.ofSeq
            )
            yield! defaultOptions |> Seq.map renderQueryOption
        ]

    let getDataPoints (options: Option seq) (defaultOptions: QueryOption seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeProtobuf (DataPointListResponse.Parser.ParseFrom >> decodeToDto)
        let request = renderRequest options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> decoder

    let getDataPointsProto (options: Option seq) (defaultOptions: QueryOption seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeProtobuf (DataPointListResponse.Parser.ParseFrom)
        let request = renderRequest options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
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
        let options' : GetDataPoints.Option seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
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
        let options' : GetDataPoints.Option seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        GetDataPoints.getDataPoints options' Seq.empty fetch Async.single

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
    let getDataPointsProto (id: int64) (options: GetDataPoints.QueryOption seq) =
        let options' : GetDataPoints.Option seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        GetDataPoints.getDataPointsProto options' Seq.empty fetch Async.single

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
    let getDataPointsMultipleAsync (options: GetDataPoints.Option seq) (defaultOptions: GetDataPoints.QueryOption seq) =
        GetDataPoints.getDataPoints options defaultOptions fetch Async.single
    
    
    /// **Description**
    ///
    /// Retrieves a list of data points from multiple time series in the same project
    /// 
    /// **Parameters**
    ///   * `options` - Sequence of `GetDataPoints.Option` describing the query for each timeseries
    ///   * `defaultOptions` - Sequence of `GetDataPoints.QueryOption` to be used as default options
    ///
    /// **Output Type**
    ///   * `Context<HttpResponseMessage> -> Async<Context<DataPointListResponse>>`
    ///
    /// **Exceptions**
    ///
    let getDataPointsMultipleProto (options: GetDataPoints.Option seq) (defaultOptions: GetDataPoints.QueryOption seq) =
        GetDataPoints.getDataPointsProto options defaultOptions fetch Async.single

[<Extension>]
type GetDataPointsExtensions =

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>Http status code.</returns>
    static member GetDataPointsAsync (this: Client, id : int64, options: GetDataPoints.QueryOption seq) : Task<DataPointListResponse> =
        task {
            let! ctx = getDataPointsProto id options this.Ctx
            match ctx.Result with
            | Ok response ->
                return response 
            | Error error ->
                let err = error2Exception error
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
    static member GetDataPointsMultipleAsync (this: Client, options: GetDataPoints.Option seq, defaultOptions: GetDataPoints.QueryOption seq) : Task<DataPointListResponse> =
        task {
            let! ctx = getDataPointsMultipleProto options defaultOptions this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
                let err = error2Exception error
                return raise err
        }

