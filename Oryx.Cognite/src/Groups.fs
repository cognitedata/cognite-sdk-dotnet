// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various asset HTTP handlers.

[<RequireQualifiedAccess>]
module Groups =
    [<Literal>]
    let Url = "/groups"

    let list (query: GroupQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<Group>> =
        source
        |> withLogMessage "Groups:list"
        |> getWithQuery query Url

    let create (items: GroupCreate seq) (source: HttpHandler<unit>) : HttpHandler<Group seq> =
        source
        |> withLogMessage "Groups:create"
        |> create items Url

    let delete (items: GroupDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Groups:delete"
        |> delete items Url
