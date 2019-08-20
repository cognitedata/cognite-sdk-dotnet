﻿namespace CogniteSdk.DataPoints

open System
open System.IO
open System.Net.Http
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.TimeSeries

[<CLIMutable>]
type DataPointCollection = {
    Id: int64
    ExternalId: string
    IsString: bool
    NumericDataPoints: seq<NumericDataPointDto>
    StringDataPoints: seq<StringDataPointDto>
}

[<RequireQualifiedAccess>]
module Latest =
    [<Literal>]
    let Url = "/timeseries/data/latest"

    type LatestRequest = {
        /// Latest point to look for datapoints, as CDF timestamp string
        Before: string option
        /// Id of timeseries
        Identity: Identity
    } with
        member this.Encoder =
            Encode.object [
                if this.Before.IsSome then
                    yield "before", Encode.string this.Before.Value
                match this.Identity with
                | CaseId id -> yield "id", Encode.int53 id
                | CaseExternalId id -> yield "externalId", Encode.string id
            ]

    type private LatestDataPointsRequest = {
        Items: seq<LatestRequest>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: LatestRequest) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type DataPointsDto = {
        Id: int64
        ExternalId: string option
        IsString: bool
        DataPoints: DataPointSeq
    } with
        static member Decoder : Decoder<DataPointsDto> =
            Decode.object (fun get ->
                let isString = get.Required.Field "isString" Decode.bool
                let dataPoints =
                    if isString then
                        get.Required.Field "datapoints" (Decode.list StringDataPointDto.Decoder) |> List.toSeq |> String
                    else
                        get.Required.Field "datapoints" (Decode.list NumericDataPointDto.Decoder) |> List.toSeq |> Numeric
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    IsString = isString
                    DataPoints = dataPoints
                })
        static member ToCollection (item : DataPointsDto) : DataPointCollection =
            let stringDataPoints, numericDataPoints =
                match item.DataPoints with
                | String data -> data, Seq.empty
                | Numeric data -> Seq.empty, data
            {
                Id = item.Id
                ExternalId = if item.ExternalId.IsSome then item.ExternalId.Value else Unchecked.defaultof<string>
                IsString = item.IsString
                NumericDataPoints = numericDataPoints
                StringDataPoints = stringDataPoints
            }

    type DataResponse = {
        Items: DataPointsDto seq
    } with
        static member Decoder : Decoder<DataResponse> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list DataPointsDto.Decoder)
                })

    let getCore (options: LatestRequest seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse DataResponse.Decoder (fun res -> res.Items)
        let request : LatestDataPointsRequest = { Items = options }

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request.Encoder)
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="options">List of requests.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of results containing the latest datapoint and ids.</returns>
    let get (queryParams: LatestRequest seq) (next: NextHandler<DataPointsDto seq,'a>) =
        getCore queryParams fetch next

    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="options">List of requests.</param>
    /// <returns>List of results containing the latest datapoint and ids.</returns>
    let getAsync (queryParams: LatestRequest seq) =
        getCore queryParams fetch Async.single


[<Extension>]
type GetLatestDataPointExtensions =
    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="options">List of tuples (id, beforeString) where beforeString describes the latest point to look for datapoints.</param>
    /// <returns>List of results containing the latest datapoint and ids.</returns>
    [<Extension>]
    static member GetLatestAsync (this: TimeSeries.DataPointsClientExtension, options: ValueTuple<Identity, string> seq, [<Optional>] token: CancellationToken) : Task<seq<DataPointCollection>> =
        async {
            let query = options |> Seq.map (fun struct (id, before) ->
                { Identity = id;
                  Before = if (isNull before) then None else Some before
                  } : Latest.LatestRequest)
            let! ctx = Latest.getAsync query this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (Latest.DataPointsDto.ToCollection)
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)

