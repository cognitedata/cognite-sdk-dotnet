namespace Fusion

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
module GetTimeseriesByIds =
    [<Literal>]
    let Url = "/timeseries/byids"

    /// Used for retrieving multiple time series
    type TimeseriesReadRequest = {
        /// Sequence of items to retrieve
        Items: seq<Identity>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: Identity) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type TimeseriesResponse = {
        Items: TimeseriesReadDto seq
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeseriesReadDto.Decoder)
            })

    let getTimeseriesByIds (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse TimeseriesResponse.Decoder (fun res -> res.Items)
        let request : TimeseriesReadRequest = {
            Items = ids
        }

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request.Encoder)
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetTimeseriesByIdsApi =
    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>The timeseries with the given ids.</returns>
    let getTimeseriesByIds (ids: seq<Identity>) (next: NextHandler<TimeseriesReadDto seq,'a>)=
        GetTimeseriesByIds.getTimeseriesByIds ids fetch next

    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <returns>The timeseries with the given ids.</returns>
    let getTimeseriesByIdsAsync (ids: seq<Identity>) =
        GetTimeseriesByIds.getTimeseriesByIds ids fetch Async.single


[<Extension>]
type GetTimeseriesByIdsExtensions =
    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <returns>The timeseries with the given ids.</returns>
    [<Extension>]
    static member GetTimeseriesByIdsAsync (this: Client, ids: seq<Identity>) : Task<seq<_>> =
        task {
            let! ctx = getTimeseriesByIdsAsync ids this.Ctx

            match ctx.Result with
            | Ok tss ->
                return tss |> Seq.map (fun ts -> ts.ToPoco ())
            | Error error ->
                let err = error2Exception error
                return raise err
        }



