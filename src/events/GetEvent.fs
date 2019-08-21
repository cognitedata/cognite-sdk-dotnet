namespace CogniteSdk.Events

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open CogniteSdk


[<RequireQualifiedAccess>]
module Entity =
    [<Literal>]
    let Url = "/events"

    let getCore (eventId: int64) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse EventReadDto.Decoder id
        let url = Url + sprintf "/%d" eventId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves information about an event given an event id. Expects a next continuation handler.
    /// </summary>
    /// <param name="assetId">The id of the event to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Event with the given id.</returns>
    let get (eventId: int64) (next: NextHandler<EventReadDto,'a>) : HttpContext -> Async<Context<'a>> =
        getCore eventId fetch next

    /// <summary>
    /// Retrieves information about an event given an event id.
    /// </summary>
    /// <param name="assetId">The id of the event to get.</param>
    /// <returns>Event with the given id.</returns>
    let getAsync (eventId: int64) : HttpContext -> Async<Context<EventReadDto>> =
        getCore eventId fetch Async.single

[<Extension>]
type GetEventClientExtensions =
    /// <summary>
    /// Retrieves information about an event given an event id.
    /// </summary>
    /// <param name="eventId">The id of the event to get.</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>Event with the given id.</returns>
    [<Extension>]
    static member GetAsync (this: ClientExtension, eventId: int64, [<Optional>] token: CancellationToken) : Task<EventReadDto> =
        async {
            let! ctx = Entity.getAsync eventId this.Ctx
            match ctx.Result with
            | Ok event ->
                return event
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
