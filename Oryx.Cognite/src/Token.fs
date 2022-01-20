// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
namespace Oryx.Cognite

open Oryx
open Oryx.Cognite
open Oryx.SystemTextJson.ResponseReader

open CogniteSdk.Token

/// Token HTTP handlers.
[<RequireQualifiedAccess>]
module Token =

    /// Returns information about the OpenID-Connect/OAuth 2 token used to access CDF resources.
    let inspect (source: HttpHandler<unit>) : HttpHandler<TokenInspect> =
        source
        |> withLogMessage "Token:inspect"
        |> GET
        |> withUrl "/api/v1/token/inspect"
        |> fetch
        |> withError decodeError
        |> json jsonOptions
        |> log
