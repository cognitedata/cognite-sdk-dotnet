namespace CogniteSdk.TimeSeries

open System.IO
open System.Net.Http

open Oryx
open Thoth.Json.Net

open CogniteSdk

[<RequireQualifiedAccess>]
module Create =
    [<Literal>]
    let Url = "/timeseries"

    type TimeseriesRequest = {
        Items: seq<WriteDto>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: WriteDto) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type TimeseriesResponse = {
        Items: ReadDto seq
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list ReadDto.Decoder)
            })

    let createCore (items: seq<WriteDto>) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let request : TimeseriesRequest = { Items = items }
        let decoder = Encode.decodeResponse TimeseriesResponse.Decoder id

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created timeseries.</returns>
    let create (items: WriteDto list) (next: NextHandler<TimeseriesResponse,'a>) =
        createCore items fetch next

    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>List of created timeseries.</returns>
    let createAsync (items: seq<WriteDto>) =
        createCore items fetch Async.single


namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Oryx
open CogniteSdk.TimeSeries

[<Extension>]
type CreateTimeSeriesClientExtensions =
    /// <summary>
    /// Create one or more new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>List of created timeseries.</returns>
    [<Extension>]
    static member CreateAsync (this: ClientExtensions.TimeSeries, items: seq<WritePoco>, [<Optional>] token: CancellationToken) : Task<ReadPoco seq> =
        async {
            let items' = items |> Seq.map WriteDto.FromPoco
            let! ctx = Create.createAsync items' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response.Items |> Seq.map (fun ts -> ts.ToPoco ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
