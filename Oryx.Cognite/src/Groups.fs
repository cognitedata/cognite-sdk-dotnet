// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk

/// Various asset HTTP handlers.

[<RequireQualifiedAccess>]
module Groups =
    [<Literal>]
    let Url = "/groups"

    let list (query: GroupQuery) : IHttpHandler<unit, ItemsWithoutCursor<Group>> =
        withLogMessage "Groups:list"
        >=> getWithQuery query Url

    let create (items: GroupCreate seq) : IHttpHandler<unit, Group seq> =
        withLogMessage "Groups:create"
        >=> create items Url

    let delete (items: GroupDelete) : IHttpHandler<unit, EmptyResponse> =
        withLogMessage "Groups:delete"
        >=> delete items Url
