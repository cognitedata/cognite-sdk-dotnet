namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.SystemTextJson.ResponseReader

open CogniteSdk
open CogniteSdk.Login

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Login =
    let get<'a, 'b> (url: string) : HttpHandler<HttpResponseMessage, 'a, 'b> =
        GET
        >=> setVersion V10
        >=> setUrl url
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions
        >=> logWithMessage "Login:get"


    /// Returns the authentication information about the asking entity.
    let status () : HttpHandler<HttpResponseMessage, LoginStatusReadDto, 'a> =
        req {
            let! data = get<LoginDataReadDto, 'a> "/login/status"
            return data.Data
        } >=> logWithMessage "Login:status"

