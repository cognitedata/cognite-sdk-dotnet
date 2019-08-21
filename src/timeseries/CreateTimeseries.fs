// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net

open CogniteSdk


[<RequireQualifiedAccess>]
module Create =
    [<Literal>]
    let Url = "/timeseries"

    type private TimeseriesRequest = {
        Items: seq<TimeSeriesWriteDto>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: TimeSeriesWriteDto) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type TimeseriesResponse = {
        Items: TimeSeriesReadDto seq
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeSeriesReadDto.Decoder)
            })

    let createCore (items: seq<TimeSeriesWriteDto>) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let request : TimeseriesRequest = { Items = items }
        let decoder = Encode.decodeResponse TimeseriesResponse.Decoder id

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created timeseries.</returns>
    let create (items: TimeSeriesWriteDto list) (next: NextFunc<TimeseriesResponse,'a>) =
        createCore items fetch next

    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>List of created timeseries.</returns>
    let createAsync (items: seq<TimeSeriesWriteDto>) =
        createCore items fetch Async.single


[<Extension>]
type CreateTimeSeriesClientExtensions =
    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>List of created timeseries.</returns>
    [<Extension>]
    static member CreateAsync (this: TimeSeriesClientExtension, items: seq<TimeSeriesEntity>, [<Optional>] token: CancellationToken) : Task<TimeSeriesEntity seq> =
        async {
            let items' = items |> Seq.map TimeSeriesWriteDto.FromTimeSeriesEntity
            let! ctx = Create.createAsync items' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response.Items |> Seq.map (fun ts -> ts.ToTimeSeriesEntity ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
