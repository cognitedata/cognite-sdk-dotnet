namespace Fusion.DataPoints

open System.IO
open System.Net.Http

open Thoth.Json.Net

open Fusion
open Fusion.Common
open Fusion.TimeSeries
open Com.Cognite.V1.Timeseries.Proto

[<RequireQualifiedAccess>]
module List =
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
        /// Start point as cdf timestamp string
        static member Start start =
            CaseStart start
        /// End point as cdf timestamp string
        static member End stop =
            CaseEnd stop
        /// Maximum number of points to return
        static member Limit limit =
            CaseLimit limit
        /// If true, include points at start or end timestamps
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

    let listCore (options: Option seq) (defaultOptions: QueryOption seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeProtobuf (DataPointListResponse.Parser.ParseFrom >> decodeToDto)
        let request = renderRequest options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> decoder

    let listProto (options: Option seq) (defaultOptions: QueryOption seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeProtobuf (DataPointListResponse.Parser.ParseFrom)
        let request = renderRequest options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    let list (id: int64) (options: QueryOption seq) (next: NextHandler<DataPoints seq,'a>) =
        let options' : Option seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        listCore options' Seq.empty fetch next

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    let listAsync (id: int64) (options: QueryOption seq) =
        let options' : Option seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        listCore options' Seq.empty fetch Async.single

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    let listProtoAsync (id: int64) (options: QueryOption seq) =
        let options' : Option seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        listProto options' Seq.empty fetch Async.single

    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries.</returns>
    let listMultiple (options: Option seq) (defaultOptions: QueryOption seq) (next: NextHandler<DataPoints seq,'a>) =
        listCore options defaultOptions fetch next

    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries.</returns>
    let listMultipleAsync (options: Option seq) (defaultOptions: QueryOption seq) =
        listCore options defaultOptions fetch Async.single


    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries as c# protobuf object.</returns>
    let listMultipleProtoAsync (options: Option seq) (defaultOptions: QueryOption seq) =
        listProto options defaultOptions fetch Async.single

namespace Fusion

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Com.Cognite.V1.Timeseries.Proto

open Fusion.DataPoints
open Fusion.Common

[<Extension>]
type GetDataPointsClientExtensions =

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    [<Extension>]
    static member GetAsync (this: Client, id : int64, options: List.QueryOption seq, [<Optional>] token: CancellationToken) : Task<DataPointListResponse> =
        async {
            let! ctx = List.listProtoAsync id options this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)

    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries.</returns>
    [<Extension>]
    static member ListMultipleAsync (this: Client, options: List.Option seq, defaultOptions: List.QueryOption seq, [<Optional>] token: CancellationToken) : Task<DataPointListResponse> =
        async {
            let! ctx = List.listMultipleProtoAsync options defaultOptions this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)

