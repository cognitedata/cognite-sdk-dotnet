// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk
open CogniteSdk.Alpha

[<RequireQualifiedAccess>]
module CdfWriter =
    [<Literal>]
    let Url = "/cdfwriter"

    let setup (request: CdfWriterSetupRequest) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "cdfwriter:setup"
        |> withAlphaHeader
        |> postV10 request (Url +/ "setup")
