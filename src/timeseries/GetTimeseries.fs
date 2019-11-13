// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries

open System
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk


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
        let query = query |> Seq.map renderOption |> List.ofSeq

        GET
        >=> setVersion V10
        >=> setResource Url
        >=> addQuery query
        >=> fetch
        >=> withError decodeError
        >=> json TimeSeriesItemsDto.Decoder

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let list (options: TimeSeriesQuery seq) (next: NextFunc<TimeSeriesItemsDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        listCore options fetch next

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let listAsync (options: TimeSeriesQuery seq) =
        listCore options fetch finishEarly

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
            let! result = Items.listAsync options ctx

            match result with
            | Ok ctx ->
                let response = ctx.Response
                let items = response.Items |> Seq.map (fun item -> item.ToTimeSeriesEntity ())
                let cursor = if response.NextCursor.IsSome then response.NextCursor.Value else Unchecked.defaultof<string>
                return { Items = items; NextCursor = cursor }
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }
