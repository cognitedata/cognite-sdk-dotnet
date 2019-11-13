// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.Decode
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

    let createCore (items: seq<TimeSeriesWriteDto>) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : TimeseriesRequest = { Items = items }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json TimeseriesResponse.Decoder

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
        createCore items fetch finishEarly


[<Extension>]
type CreateTimeSeriesClientExtensions =
    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>List of created timeseries.</returns>
    [<Extension>]
    static member CreateAsync (this: ClientExtension, items: seq<TimeSeriesEntity>, [<Optional>] token: CancellationToken) : Task<TimeSeriesEntity seq> =
        task {
            let items' = items |> Seq.map TimeSeriesWriteDto.FromTimeSeriesEntity
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Create.createAsync items' ctx
            match result with
            | Ok ctx ->
                return ctx.Response.Items |> Seq.map (fun ts -> ts.ToTimeSeriesEntity ())
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }
