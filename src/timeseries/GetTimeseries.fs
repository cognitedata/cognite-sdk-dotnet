namespace CogniteSdk.TimeSeries

open System
open System.IO
open System.Net.Http

open Oryx
open Thoth.Json.Net
open CogniteSdk


[<RequireQualifiedAccess>]
module List =
    [<Literal>]
    let Url = "/timeseries"

    type TimeseriesResponse = {
        Items: ReadDto seq
        NextCursor: string option
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list ReadDto.Decoder)
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

    let listCore (query: Option seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse<TimeseriesResponse, TimeseriesResponse, 'a> TimeseriesResponse.Decoder id
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
    let list (options: Option seq) (next: NextHandler<TimeseriesResponse,'a>) : HttpContext -> Async<Context<'a>> =
        listCore options fetch next

    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    let listAsync (options: Option seq) =
        listCore options fetch Async.single

namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Oryx
open CogniteSdk.TimeSeries


[<Extension>]
type ListTimeseriesClientExtensions =
    /// <summary>
    /// Retrieves a list of all time series in a project. Parameters can be used to select a subset of time series.
    /// This operation supports pagination.
    /// </summary>
    /// <param name="options">Timeseries lookup options.</param>
    /// <returns>The timeseries with the given id and an optional cursor.</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtensions.TimeSeries, options: List.Option seq, [<Optional>] token: CancellationToken) : Task<_> =
        async {
            let! ctx = List.listAsync options this.Ctx

            match ctx.Result with
            | Ok response ->
                return {|
                        Items = response.Items |> Seq.map (fun item -> item.ToPoco ())
                        NextCursor = response.NextCursor
                    |}
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
