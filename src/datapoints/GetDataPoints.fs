// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Com.Cognite.V1.Timeseries.Proto
open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk

/// Query parameters
type DataPointQuery =
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

type DefaultOption = DataPointQuery

[<CLIMutable>]
type DataPointMultipleQuery = {
    Id: Identity
    QueryOptions: DataPointQuery seq
}

[<RequireQualifiedAccess>]
module Items =
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
                        data.NumericDatapoints.Datapoints |> Seq.map (NumericDataPointDto.FromProtobuf) |> Numeric
                    | _ ->
                        Seq.empty |> Numeric
            }

    let decodeToDto (data : DataPointListResponse) : seq<DataPoints> =
        data.Items |> Seq.map (DataPoints.FromProto)

    type DataResponse = {
        Items: DataPoints seq
    }

    let renderQueryOption (option: DataPointQuery) : string*Thoth.Json.Net.JsonValue =
        match option with
        | CaseStart start -> "start", Encode.string start
        | CaseEnd end'  -> "end", Encode.string end'
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseIncludeOutsidePoints iop -> "includeOutsidePoints", Encode.bool iop

    let renderRequest (options: DataPointMultipleQuery seq) (defaultOptions: DataPointQuery seq) =
        Encode.object [
            yield "items", Encode.list (
                options
                |> Seq.map (fun (option: DataPointMultipleQuery) ->
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

    let listCore (options: DataPointMultipleQuery seq) (defaultOptions: DataPointQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request = renderRequest options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> withError decodeError
        >=> protobuf (DataPointListResponse.Parser.ParseFrom >> decodeToDto)

    let listProto (options: DataPointMultipleQuery seq) (defaultOptions: DataPointQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request = renderRequest options defaultOptions

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request)
        >=> setResponseType Protobuf
        >=> fetch
        >=> withError decodeError
        >=> protobuf DataPointListResponse.Parser.ParseFrom

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    let list (id: int64) (options: DataPointQuery seq) (next: NextFunc<DataPoints seq,'a>) =
        let options' : DataPointMultipleQuery seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        listCore options' Seq.empty fetch next

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    let listAsync (id: int64) (options: DataPointQuery seq) =
        let options' : DataPointMultipleQuery seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        listCore options' Seq.empty fetch finishEarly

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    let listProtoAsync (id: int64) (options: DataPointQuery seq) =
        let options' : DataPointMultipleQuery seq = Seq.singleton { Id = Identity.Id id; QueryOptions = options }
        listProto options' Seq.empty fetch finishEarly

    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries.</returns>
    let listMultiple (options: DataPointMultipleQuery seq) (defaultOptions: DataPointQuery seq) (next: NextFunc<DataPoints seq,'a>) =
        listCore options defaultOptions fetch next

    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries.</returns>
    let listMultipleAsync (options: DataPointMultipleQuery seq) (defaultOptions: DataPointQuery seq) =
        listCore options defaultOptions fetch finishEarly


    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries as c# protobuf object.</returns>
    let listMultipleProtoAsync (options: DataPointMultipleQuery seq) (defaultOptions: DataPointQuery seq) =
        listProto options defaultOptions fetch finishEarly

[<Extension>]
type GetDataPointsClientExtensions =

    /// <summary>
    /// Retrieves a list of data points from single time series in the same project.
    /// </summary>
    /// <param name="id">Id of timeseries to query for datapoints. </param>
    /// <param name="options">Options describing a query for datapoints.</param>
    /// <returns>A single datapoint response object containing a list of datapoints.</returns>
    [<Extension>]
    static member GetAsync (this: ClientExtension, id : int64, options: DataPointQuery seq, [<Optional>] token: CancellationToken) : Task<DataPointListResponse> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listProtoAsync id options ctx
            match result with
            | Ok ctx ->
                return ctx.Response
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="options">Parameters describing a query for multiple datapoints.</param>
    /// <param name="defaultOptions">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <returns>List of datapoint responses containing lists of datapoints for each timeseries.</returns>
    [<Extension>]
    static member ListMultipleAsync (this: ClientExtension, options: DataPointMultipleQuery seq, defaultOptions: DataPointQuery seq, [<Optional>] token: CancellationToken) : Task<DataPointListResponse> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listMultipleProtoAsync options defaultOptions ctx
            match result with
            | Ok ctx ->
                return ctx.Response
            | Error error -> return raiseError error
        }

