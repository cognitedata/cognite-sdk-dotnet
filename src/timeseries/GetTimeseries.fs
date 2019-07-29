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

[<RequireQualifiedAccess>]
module GetTimeseries =
    [<Literal>]
    let Url = "/timeseries"

    type TimeseriesResponse = {
        Items: TimeseriesReadDto seq
        NextCursor: string option
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeseriesReadDto.Decoder)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    // Get parameters
    type Option =
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
        
    let renderOption (option: Option) =
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

    let getTimeseries (query: Option seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse<TimeseriesResponse, TimeseriesResponse, 'a> TimeseriesResponse.Decoder id
        let query = query |> Seq.map renderOption |> List.ofSeq

        GET
        >=> setVersion V10
        >=> setResource Url
        >=> addQuery query
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetTimeseriesApi =
    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let getTimeseries (options: GetTimeseries.Option seq) (next: NextHandler<GetTimeseries.TimeseriesResponse,'a>) : HttpContext -> Async<Context<'a>> =
        GetTimeseries.getTimeseries options fetch next

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let getTimeseriesAsync (options: GetTimeseries.Option seq) =
        GetTimeseries.getTimeseries options fetch Async.single

[<Extension>]
type GetTimeseriesExtensions =
    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    [<Extension>]
    static member GetTimeseriesAsync (this: Client) (options: GetTimeseries.Option seq) : Task<_> =
        task {
            let! ctx = getTimeseriesAsync options this.Ctx

            match ctx.Result with
            | Ok response ->
                return {|
                        Items = response.Items |> Seq.map (fun item -> item.ToPoco ())
                        NextCursor = response.NextCursor
                    |}
            | Error error ->
                let err = error2Exception error
                return raise err
        }
