namespace Fusion

open System
open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Common
open Fusion.Api
open Fusion.Timeseries

[<RequireQualifiedAccess>]
module CreateTimeseries =
    [<Literal>]
    let Url = "/timeseries"

    type TimeseriesRequest = {
        Items: seq<TimeseriesWriteDto>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: TimeseriesWriteDto) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type TimeseriesResponse = {
        Items: TimeseriesReadDto seq
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeseriesReadDto.Decoder)
            })

    let createTimeseries (items: seq<TimeseriesWriteDto>) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let request : TimeseriesRequest = { Items = items }
        let decoder = decodeResponse TimeseriesResponse.Decoder id

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module CreateTimeseriesApi =
    /// **Description**
    ///
    /// Create new timeseries
    ///
    /// **Parameters**
    ///   * `items` - The list of timeseries to create.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let createTimeseries (items: TimeseriesWriteDto list) (next: NextHandler<CreateTimeseries.TimeseriesResponse,'a>) =
        CreateTimeseries.createTimeseries items fetch next

    let createTimeseriesAsync (items: seq<TimeseriesWriteDto>) =
        CreateTimeseries.createTimeseries items fetch Async.single

[<Extension>]
type CreateTimeseriesExtensions =
    /// <summary>
    /// Create a new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member CreateTimeseriesAsync (this: Client) (items: seq<TimeseriesWritePoco>) : Task<TimeseriesReadPoco seq> =
        task {
            let items' = items |> Seq.map TimeseriesWriteDto.FromPoco
            let! ctx = createTimeseriesAsync items' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response.Items |> Seq.map (fun ts -> ts.ToPoco ())
            | Error error ->
                let err = error2Exception error
                return raise err
        }
