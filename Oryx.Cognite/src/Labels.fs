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
    let list (query: LabelQuery) : IHttpHandler<unit, ItemsWithCursor<Label>> =
        withLogMessage "Labels:get"
        >=> list query Url

    /// Create new label definitions in the given project. Returns list of created labels.
    let create (items: LabelCreate seq) : IHttpHandler<unit, Label seq> =
        withLogMessage "Labels:create"
        >=> create items Url

    /// Delete multiple label definitions.
    let delete (items: LabelDelete) : IHttpHandler<unit, EmptyResponse> =
        withLogMessage "Labels:delete"
        >=> delete items Url

