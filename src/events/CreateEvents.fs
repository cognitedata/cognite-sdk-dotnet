namespace CogniteSdk.Events

open System.IO
open System.Net.Http

open Thoth.Json.Net
open CogniteSdk
open Oryx



[<RequireQualifiedAccess>]
module Create =
    [<Literal>]
    let Url = "/events"

    type Request = {
        Items: WriteDto seq
    } with
         member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (it: WriteDto) -> it.Encoder) this.Items |> Encode.seq
            ]

    type EventResponse = {
        Items: ReadDto seq
    } with
         static member Decoder : Decoder<EventResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list ReadDto.Decoder |> Decode.map seq)
            })

    let createCore (events: WriteDto seq) (fetch: HttpHandler<HttpResponseMessage,Stream,'a>)  =
        let decoder = Encode.decodeResponse EventResponse.Decoder (fun res -> res.Items)
        let request : Request = { Items = events }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

    /// <summary>
    /// Create one or more events.
    /// </summary>
    /// <param name="events">List of events to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created events.</returns>
    let create (events: WriteDto seq) (next: NextHandler<ReadDto seq, 'a>) =
        createCore events fetch next

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let createAsync (events: WriteDto seq) =
        createCore events fetch Async.single

namespace Fusion

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices

open CogniteSdk.Events
open CogniteSdk
open System.Threading

[<Extension>]
type CreateEventExtensions =
    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    [<Extension>]
    static member CreateAsync (this: ClientExtensions.Events, events: Event seq, [<Optional>] token: CancellationToken) : Task<Event seq> =
        async {
            let events' = events |> Seq.map WriteDto.FromEvent
            let! ctx = Create.createAsync events' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun event -> event.ToEvent ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
