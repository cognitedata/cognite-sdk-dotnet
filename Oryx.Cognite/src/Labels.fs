// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various label http handlers
[<RequireQualifiedAccess>]
module Labels =
    [<Literal>]
    let Url = "/labels"

    /// List label definitions with pagination and an optional filter.
    let list (query: LabelQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Label>> =
        source
        |> withLogMessage "Labels:get"
        |> list query Url

    /// Create new label definitions in the given project. Returns list of created labels.
    let create (items: LabelCreate seq) (source: HttpHandler<unit>) : HttpHandler<Label seq> =
        source
        |> withLogMessage "Labels:create"
        |> create items Url

    /// Delete multiple label definitions.
    let delete (items: LabelDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Labels:delete"
        |> delete items Url
