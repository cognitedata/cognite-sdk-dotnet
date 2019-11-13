// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Login

open System
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.Decode

open CogniteSdk

/// The functional login status core module
[<RequireQualifiedAccess>]
module Status =
    [<Literal>]
    let Url = "/login/status"

    let statusCore (fetch: HttpHandler<HttpResponseMessage, 'a>) =

        GET
        >=> setVersion V10
        >=> setUrl Url
        >=> fetch
        >=> withError decodeError
        >=> json LoginStatusItemsDto.Decoder
        >=> map (fun items -> items.Data)

    /// <summary>
    /// Returns the authentication information about the asking entity.
    /// </summary>
    ///
    /// <returns>List of events matching given criteria.</returns>
    let status (next: NextFunc<LoginStatusDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        statusCore fetch next

    /// <summary>
    /// Returns the authentication information about the asking entity.
    /// </summary>
    ///
    /// <returns>List of events matching given criteria.</returns>
    let statusAsync : HttpContext -> HttpFuncResult<LoginStatusDto> =
        statusCore fetch finishEarly

[<Extension>]
type LoginStatusClientExtensions =
    /// <summary>
    /// Returns the authentication information about the asking entity.
    /// </summary>
    ///
    /// <param name="token">Cancellation tokenb to use.</param>
    ///
    /// <returns>The login status.</returns>
    [<Extension>]
    static member StatusAsync (this: ClientExtension, token: CancellationToken) : Task<LoginStatusEntity> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Status.statusAsync ctx
            match result with
            | Ok ctx ->
                let status = ctx.Response
                return status.ToLoginStatusEntity ()
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

