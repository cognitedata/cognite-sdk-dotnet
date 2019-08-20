namespace CogniteSdk.TimeSeries

open System
open System.IO
open System.Net.Http

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
module TimeSeries =
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

    let listCore (query: TimeSeriesQuery seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse<TimeSeriesItemsDto, TimeSeriesItemsDto, 'a> TimeSeriesItemsDto.Decoder id
        let query = query |> Seq.map renderOption |> List.ofSeq

        GET
        >=> setVersion V10
        >=> setResource Url
        >=> addQuery query
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let list (options: TimeSeriesQuery seq) (next: NextHandler<TimeSeriesItemsDto,'a>) : HttpContext -> Async<Context<'a>> =
        listCore options fetch next

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let listAsync (options: TimeSeriesQuery seq) =
        listCore options fetch Async.single

namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Oryx
open CogniteSdk.TimeSeries

[<CLIMutable>]
type TimeSeriesItems = {
    Items: TimeSeriesEntity seq
    NextCursor: string
}

[<Extension>]
type ListTimeseriesClientExtensions =
    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    [<Extension>]
    static member ListAsync (this: TimeSeriesClientExtension, options: TimeSeries.TimeSeriesQuery seq, [<Optional>] token: CancellationToken) : Task<_> =
        async {
            let! ctx = TimeSeries.listAsync options this.Ctx

            match ctx.Result with
            | Ok response ->
                let items = response.Items |> Seq.map (fun item -> item.ToEntity ())
                let cursor = if response.NextCursor.IsSome then response.NextCursor.Value else Unchecked.defaultof<string>
                return { Items = items; NextCursor = cursor }
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
