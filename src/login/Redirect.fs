// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Login

open System
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx

open CogniteSdk

/// The functional event search core module
[<RequireQualifiedAccess>]
module Redirect =
    [<Literal>]
    let Url = "/login/redirect"

    let extractHeader (header: string) (next: NextFunc<_,_>) (context: HttpContext) = task {
        match context.Result with
        | Ok response ->
            let (success, values) = response.Headers.TryGetValues header
            let values = if (isNull values) then [] else values |> List.ofSeq
            match (success, values ) with
            | (true, value :: _) ->
                return! next { Request = context.Request; Result = Ok value }
            | _ ->
                return! next { Request = context.Request; Result = Error { ResponseError.empty with Message = sprintf "Missing header: %s" header }}
        | Error error -> return! next { Request = context.Request; Result = Error error }
    }
    let redirectCore (project: string) (redirectUrl: Uri) (errorRedirectUrl: Uri option) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let query = [
            yield "project", project
            yield "redirectUrl", redirectUrl.ToString ()
            if errorRedirectUrl.IsSome then
                yield "errorRedirectUrl", errorRedirectUrl.Value.ToString ()
        ]

        GET
        >=> addQuery query
        >=> setVersion V10
        >=> setUrl Url
        >=> fetch
        >=> extractHeader "location"

    /// <summary>
    /// Retrieves a list of events matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of events matching given criteria.</returns>
    let redirect (project: string) (redirectUri: Uri) (errorRedirectUrl: Uri option) (next: NextFunc<string,'a>) : HttpContext -> Task<Context<'a>> =
        redirectCore project redirectUri errorRedirectUrl fetch next

    /// <summary>
    /// Retrieves a list of events matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of events matching given criteria.</returns>
    let redirectAsync (project: string) (redirectUri: Uri) (errorRedirectUrl: Uri option) : HttpContext -> Task<Context<string>> =
        redirectCore project redirectUri errorRedirectUrl fetch Task.FromResult

[<Extension>]
type LoginRedirectClientExtensions =
    /// <summary>
    /// Get the redirect for a login URL.
    /// </summary>
    ///
    /// <param name="project">The CDF project to get redirect URL for.</param>
    /// <param name="redirectUrl">The url to send the user to after the login is successful.</param>
    /// <param name="errorRedirectUrl">The url to send the user to if the login fails or is aborted. If this is not passed in, the value of the redirectUrl will be used.</param>
    /// <param name="token">Cancellation tokenb to use.</param>
    ///
    /// <returns>The login URL.</returns>
    [<Extension>]
    static member RedirectAsync (this: ClientExtension, project: string, redirectUrl: Uri, errorRedirectUrl: Uri, token: CancellationToken) : Task<string> =
        task {
            let errorRedirectUri = if not (isNull errorRedirectUrl) then Some errorRedirectUrl else None
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx = Redirect.redirectAsync project redirectUrl errorRedirectUri ctx
            match ctx.Result with
            | Ok location ->
                return location
            | Error error ->
                return raise (error.ToException ())
        }

    /// <summary>
    /// Get the redirect for a login URL.
    /// </summary>
    ///
    /// <param name="project">The CDF project to get redirect URL for.</param>
    /// <param name="redirectUrl">The url to send the user to after the login is successful.</param>
    /// <param name="errorRedirectUrl">The url to send the user to if the login fails or is aborted. If this is not passed in, the value of the redirectUrl will be used.</param>
    ///
    /// <returns>The login URL.</returns>
    [<Extension>]
    static member RedirectAsync (this: ClientExtension, project: string, redirectUrl: Uri, errorRedirectUrl: Uri) : Task<string> =
        this.RedirectAsync(project, redirectUrl, errorRedirectUrl, CancellationToken.None)

    /// <summary>
    /// Get the redirect for a login URL.
    /// </summary>
    ///
    /// <param name="project">The CDF project to get redirect URL for.</param>
    /// <param name="redirectUrl">The url to send the user to after the login is successful.</param>
    /// <param name="token">Cancellation tokenb to use.</param>
    ///
    /// <returns>The login URL.</returns>
    [<Extension>]
    static member RedirectAsync (this: ClientExtension, project: string, redirectUrl: Uri, token: CancellationToken) : Task<string> =
        this.RedirectAsync(project, redirectUrl, null, token)

    /// <summary>
    /// Get the redirect for a login URL.
    /// </summary>
    ///
    /// <param name="project">The CDF project to get redirect URL for.</param>
    /// <param name="redirectUrl">The url to send the user to after the login is successful.</param>
    ///
    /// <returns>The login URL.</returns>
    [<Extension>]
    static member RedirectAsync (this: ClientExtension, project: string, redirectUrl: Uri) : Task<string> =
        this.RedirectAsync(project, redirectUrl, null, CancellationToken.None)