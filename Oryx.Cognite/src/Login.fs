namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk
open CogniteSdk.Login

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Login =
    [<Literal>]
    let Url = "/login"

    /// Returns the authentication information about the asking entity.
    let status () : HttpHandler<HttpResponseMessage, LoginStatusReadDto, 'a> =
        req {
            let! data = get<LoginDataReadDto, 'a> (Url +/ "status")
            return data.Data
        }
