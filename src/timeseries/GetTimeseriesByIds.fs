namespace CogniteSdk.TimeSeries

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.TimeSeries


[<RequireQualifiedAccess>]
module GetByIds =
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
        Items: TimeSeriesReadDto seq
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeSeriesReadDto.Decoder)
            })

    let getByIdsCore (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse TimeseriesResponse.Decoder (fun res -> res.Items)
        let request : TimeseriesReadRequest = {
            Items = ids
        }

        POST
        >=> setVersion V10
        >=> setResource Url
        >=> setContent (Content.JsonValue request.Encoder)
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>The timeseries with the given ids.</returns>
    let getByIds (ids: seq<Identity>) (next: NextHandler<TimeSeriesReadDto seq,'a>)=
        getByIdsCore ids fetch next

    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <returns>The timeseries with the given ids.</returns>
    let getByIdsAsync (ids: seq<Identity>) =
        getByIdsCore ids fetch Async.single

[<Extension>]
type GetTimeseriesByIdsClientExtensions =
    /// <summary>
    /// Retrieves information about multiple timeseries in the same project.
    /// A maximum of 1000 timeseries IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the timeseries to get.</param>
    /// <returns>The timeseries with the given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: TimeSeriesClientExtension, ids: seq<Identity>, [<Optional>] token: CancellationToken) : Task<seq<_>> =
        async {
            let! ctx = GetByIds.getByIdsAsync ids this.Ctx

            match ctx.Result with
            | Ok tss ->
                return tss |> Seq.map (fun ts -> ts.ToTimeSeriesEntity ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)



