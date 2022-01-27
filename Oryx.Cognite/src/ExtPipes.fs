// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various extraction pipeline HTTP handlers
[<RequireQualifiedAccess>]
module ExtPipes =
    [<Literal>]
    let Url = "/extpipes"

    let RunsUrl = Url +/ "runs"

    /// Retrieves list of extraction pipelines matching filter, and a cursor if given limit is exceeded.
    let list (query: ExtPipeQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<ExtPipe>> =
        source
        |> withLogMessage "ExtPipes:get"
        |> list query Url

    /// Create new extraction pipelines in the given project. Returns list of created extraction pipelines.
    let create (items: ExtPipeCreate seq) (source: HttpHandler<unit>) : HttpHandler<ExtPipe seq> =
        source
        |> withLogMessage "ExtPipes:create"
        |> create items Url

    /// Delete multiple extraction pipelines.
    /// Returns an empty response, not JSON.
    let delete (items: ExtPipeDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "ExtPipes:delete"
        |> delete items Url

    /// Update extraction pipelines
    let update (query: IEnumerable<UpdateItem<ExtPipeUpdate>>) (source: HttpHandler<unit>) : HttpHandler<ExtPipe seq> =
        source
        |> withLogMessage "ExtPipes:update"
        |> update query Url

    /// Retrieve a list of extraction pipelines. Optionally ignore unknown ids.
    let retrieve
        (ids: Identity seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<ExtPipe seq> =
        source
        |> withLogMessage "ExtPipes:retrieve"
        |> retrieveIgnoreUnknownIds ids (Option.ofNullable ignoreUnknownIds) Url

    /// Retrieves list of extraction pipeline runs matching filter, and a cursor if given limit is exceeded.
    let listRuns (query: ExtPipeRunQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<ExtPipeRun>> =
        source
        |> withLogMessage "ExtPipeRuns:get"
        |> HttpHandler.list query RunsUrl

    /// Create new extraction pipeline runs in the given project. Returns list of created extraction pipeline runs.
    let createRuns (items: ExtPipeRunCreate seq) (source: HttpHandler<unit>) : HttpHandler<ExtPipeRun seq> =
        source
        |> withLogMessage "ExtPipeRuns:create"
        |> HttpHandler.create items RunsUrl
