namespace Cognite.Sdk

open System
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Api
open Cognite.Sdk.Timeseries

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

    let createTimeseries (items: seq<TimeseriesWriteDto>) (fetch: HttpHandler<HttpResponseMessage, string, 'a>) =
        let request : TimeseriesRequest = { Items = items }
        let decoder = decodeResponse TimeseriesResponse.Decoder id
        let body = Encode.stringify request.Encoder

        POST
        >=> setVersion V10
        >=> setBody body
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

    let createTimeseriesAsync (items: seq<TimeseriesWriteDto>) (fetch: HttpHandler<HttpResponseMessage, string, CreateTimeseries.TimeseriesResponse>) =
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
            let! ctx = createTimeseriesAsync items' fetch this.Ctx
            match ctx.Result with
            | Ok response ->
                return response.Items |> Seq.map (fun ts -> ts.ToPoco ())
            | Error error ->
               return raise (Error.error2Exception error)
        }
