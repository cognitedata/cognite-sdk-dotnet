// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// handler functions for the Alpha SDK.
namespace Oryx.Cognite.Alpha


open Oryx
open Oryx.Cognite

/// Oryx HTTP handlers for specific use within the Cognite SDK
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Handler =
    let withAlphaHeader<'T> (source: HttpHandler<'T>) : HttpHandler<'T> =
        source |> withHeader ("cdf-version", "alpha")

    let withAlphaVersion<'TSource> (source: HttpHandler<'TSource>) : HttpHandler<'TSource> =
        source |> withAlphaHeader<'TSource> |> withVersion V10
