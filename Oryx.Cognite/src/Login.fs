// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
namespace Oryx.Cognite

open Oryx
open Oryx.Cognite
open Oryx.SystemTextJson.ResponseReader

open CogniteSdk.Login

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Login =
    let get<'TResult> (url: string) : IHttpHandler<unit, 'TResult> =
        GET
        >=> withVersion V10
        >=> withUrl url
        >=> withLogMessage "Login:get"
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions
        >=> log

    /// Returns the authentication information about the asking entity.
    let status () : IHttpHandler<unit, LoginStatus> =
        withLogMessage "Login:status"
        >=> req {
            let! data = get<LoginDataRead> "/login/status"
            return data.Data
        }
