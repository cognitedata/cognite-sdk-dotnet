// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open CogniteSdk
open FSharp.Control.Tasks.V2.ContextInsensitive


[<RequireQualifiedAccess>]
module Entity =
    [<Literal>]
    let Url = "/events"

    let getCore (eventId: int64) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let url = Url + sprintf "/%d" eventId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> withError decodeError
        >=> json EventReadDto.Decoder

    /// <summary>
    /// Retrieves information about an event given an event id. Expects a next continuation handler.
    /// </summary>
    /// <param name="assetId">The id of the event to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Event with the given id.</returns>
    let get (eventId: int64) (next: NextFunc<EventReadDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        getCore eventId fetch next

    /// <summary>
    /// Retrieves information about an event given an event id.
    /// </summary>
    /// <param name="assetId">The id of the event to get.</param>
    /// <returns>Event with the given id.</returns>
    let getAsync (eventId: int64) : HttpContext -> HttpFuncResult<EventReadDto> =
        getCore eventId fetch finishEarly

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
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Entity.getAsync eventId ctx
            match result with
            | Ok ctx ->
                return ctx.Response
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }
