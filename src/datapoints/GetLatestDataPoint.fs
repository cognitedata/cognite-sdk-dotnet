// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints

open System
open System.Net.Http
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.ResponseReaders
open Thoth.Json.Net

open CogniteSdk

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
                        get.Required.Field "datapoints" (Decode.list StringDataPointDto.Decoder)
                        |> List.toSeq
                        |> DataPointSeq.String
                    else
                        get.Required.Field "datapoints" (Decode.list NumericDataPointDto.Decoder)
                        |> List.toSeq
                        |> DataPointSeq.Numeric
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    IsString = isString
                    DataPoints = dataPoints
                })
        static member ToCollection (item : DataPointsDto) : DataPointCollection =
            let stringDataPoints, numericDataPoints =
                match item.DataPoints with
                | DataPointSeq.String data -> data, Seq.empty
                | DataPointSeq.Numeric data -> Seq.empty, data
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

    let getCore (options: LatestRequest seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : LatestDataPointsRequest = { Items = options }

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request.Encoder)
        >=> fetch
        >=> withError decodeError
        >=> json DataResponse.Decoder
        >=> map (fun res -> res.Items)

    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="options">List of requests.</param>
    /// <param name="next">The next HTTP handler.</param>
    /// <returns>List of results containing the latest datapoint and ids.</returns>
    let get (queryParams: LatestRequest seq) (next: NextFunc<DataPointsDto seq,'a>) =
        getCore queryParams fetch next

    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="options">List of requests.</param>
    /// <returns>List of results containing the latest datapoint and ids.</returns>
    let getAsync (queryParams: LatestRequest seq) : HttpContext -> HttpFuncResult<seq<DataPointsDto>> =
        getCore queryParams fetch finishEarly


[<Extension>]
type GetLatestDataPointExtensions =
    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="options">List of tuples (id, beforeString) where beforeString describes the latest point to look for datapoints.</param>
    /// <returns>List of results containing the latest datapoint and ids.</returns>
    [<Extension>]
    static member GetLatestAsync (this: ClientExtension, options: ValueTuple<Identity, string> seq, [<Optional>] token: CancellationToken) : Task<seq<DataPointCollection>> =
        task {
            let query = options |> Seq.map (fun struct (id, before) ->
                let before' = if (isNull before) then None else Some before
                { Identity = id;
                  Before = before' } : Latest.LatestRequest)

            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Latest.getAsync query ctx
            match result with
            | Ok ctx ->
                return ctx.Response |> Seq.map (Latest.DataPointsDto.ToCollection)
            | Error error -> return raiseError error
        }

