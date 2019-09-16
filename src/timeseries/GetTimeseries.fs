// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries

open System
open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net

open CogniteSdk
open FSharp.Control.Tasks.V2.ContextInsensitive


// Get parameters
type TimeSeriesQuery =
    private // Expose members instead for C# interoperability
    | CaseLimit of int32
    | CaseIncludeMetaData of bool
    | CaseCursor of string
    | CaseAssetIds of int64 seq
    | CaseRootAssetIds of int64 seq

    /// Maximum number of results to return
    static member Limit limit =
        CaseLimit limit
    /// If true, include meta data of each timeseries
    static member IncludeMetaData imd =
        CaseIncludeMetaData imd
    /// Cursor returned from previous query
    static member Cursor cursor =
        CaseCursor cursor
    /// Filter out timeseries without assetId in this list
    static member AssetIds ids =
        CaseAssetIds ids
    /// Filter out timeseries without rootAssetId in this list
    static member RootAssetIds ids =
        CaseRootAssetIds ids

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let Url = "/timeseries"

    type TimeSeriesItemsDto = {
        Items: TimeSeriesReadDto seq
        NextCursor: string option
    } with
        static member Decoder : Decoder<TimeSeriesItemsDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeSeriesReadDto.Decoder)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })


    let renderOption (option: TimeSeriesQuery) =
        match option with
        | CaseLimit limit -> "limit", limit.ToString ()
        | CaseIncludeMetaData imd -> "includeMetadata", imd.ToString().ToLower()
        | CaseCursor cursor -> "cursor", cursor
        | CaseAssetIds ids ->
            let list = ids |> Seq.map (fun a -> a.ToString ()) |> seq<string>
            "assetIds", sprintf "[%s]" (String.Join (",", list))
        | CaseRootAssetIds ids ->
            let list = ids |> Seq.map (fun a -> a.ToString ()) |> seq<string>
            "rootAssetIds", sprintf "[%s]" (String.Join (",", list))

    let listCore (query: TimeSeriesQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse<TimeSeriesItemsDto, TimeSeriesItemsDto, 'a> TimeSeriesItemsDto.Decoder id
        let query = query |> Seq.map renderOption |> List.ofSeq

        GET
        >=> setVersion V10
        >=> setResource Url
        >=> addQuery query
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let list (options: TimeSeriesQuery seq) (next: NextFunc<TimeSeriesItemsDto,'a>) : HttpContext -> Task<Context<'a>> =
        listCore options fetch next

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let listAsync (options: TimeSeriesQuery seq) =
        listCore options fetch Task.FromResult

[<Extension>]
type ListTimeseriesClientExtensions =
    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: TimeSeriesQuery seq, [<Optional>] token: CancellationToken) : Task<_> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx = Items.listAsync options ctx

            match ctx.Result with
            | Ok response ->
                let items = response.Items |> Seq.map (fun item -> item.ToTimeSeriesEntity ())
                let cursor = if response.NextCursor.IsSome then response.NextCursor.Value else Unchecked.defaultof<string>
                return { Items = items; NextCursor = cursor }
            | Error error ->
                return raise (error.ToException ())
        }
