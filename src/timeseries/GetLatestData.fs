namespace Cognite.Sdk

open System
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common
open Cognite.Sdk.Timeseries

[<RequireQualifiedAccess>]
module GetLatestData =
    [<Literal>]
    let Url = "/timeseries/data/latest"

    type LatestDataRequest = {
        Before: string option
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

    type TimeseriesLatestRequest = {
        Items: seq<LatestDataRequest>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: LatestDataRequest) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type DataPoints = {
        Id: int64
        ExternalId: string option
        IsString: bool
        DataPoints: DataPointDto seq
    } with
        static member Decoder : Decoder<DataPoints> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "exteralId" Decode.string
                    IsString = get.Required.Field "isString" Decode.bool
                    DataPoints = get.Required.Field "datapoints" (Decode.list DataPointDto.Decoder)
                })

    type DataResponse = {
        Items: DataPoints seq
    } with
        static member Decoder : Decoder<DataResponse> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list DataPoints.Decoder)
                })

    // Get parameters
    type Option =
        private // Expose members instead for C# interoperability
        | CaseLimit of int32
        | CaseIncludeMetaData of bool
        | CaseCursor of string
        | CaseAssetIds of int64 seq

        static member Limit limit =
            CaseLimit limit
        static member IncludeMetaData imd =
            CaseIncludeMetaData imd
        static member Cursor cursor =
            CaseCursor cursor
        static member AssetIds ids =
            CaseAssetIds ids

    let renderOption (option: Option) =
        match option with
        | CaseLimit limit -> "limit", limit.ToString ()
        | CaseIncludeMetaData imd -> "includeMetadata", imd.ToString().ToLower()
        | CaseCursor cursor -> "cursor", cursor
        | CaseAssetIds ids ->
            let list = ids |> Seq.map (fun a -> a.ToString ()) |> seq<string>
            "assetIds", sprintf "[%s]" (String.Join (",", list))


    let getTimeseriesLatestData (options: LatestDataRequest seq) (fetch: HttpHandler<HttpResponseMessage, string, 'a>) =
        let decoder = decodeResponse DataResponse.Decoder (fun res -> res.Items)
        let request : TimeseriesLatestRequest = { Items = options }
        let body = request.Encoder |> Encode.stringify

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setBody body
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetLatestDataApi =
        /// **Description**
    ///
    /// Retrieves the single latest data point in a time series.
    ///
    /// **Parameters**
    ///   * `queryParams` - parameter of type `seq<QueryLatestParam>`
    ///   * `ctx` - parameter of type `HttpContext`
    ///
    /// **Output Type**
    ///   * `Async<Context<seq<PointResponseDataPoints>>>`
    ///
    let getTimeseriesLatestData (queryParams: GetLatestData.LatestDataRequest seq) (next: NextHandler<GetLatestData.DataPoints seq,'a>) =
        GetLatestData.getTimeseriesLatestData queryParams fetch next


    let getTimeseriesLatestDataAsync (queryParams: GetLatestData.LatestDataRequest seq) =
        GetLatestData.getTimeseriesLatestData queryParams fetch Async.single


[<Extension>]
type GetLatestDataExtensions =
    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="queryParams">Instance of QueryDataLatest object with query parameters.</param>
    /// <param name="client">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member GetTimeseriesLatestDataAsync (this: Client) (options: ValueTuple<Identity, string> seq) : Task<seq<GetLatestData.DataPoints>> =
        task {
            let query = options |> Seq.map (fun struct (id, before) ->
                { Identity = id;
                  Before = if before = null then None else Some before
                  } : GetLatestData.LatestDataRequest)
            let! ctx = getTimeseriesLatestDataAsync query this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

