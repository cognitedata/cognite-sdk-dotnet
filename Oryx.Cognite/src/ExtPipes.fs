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

    let ConfigsUrl = Url +/ "config"

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

    /// BETA: Create a new configuration revision for the given extraction pipeline.
    let createConfig (item: CreateExtPipeConfig) (source: HttpHandler<unit>) : HttpHandler<ExtPipeConfig> =
        source
        |> withVersion ApiVersion.Playground
        |> withLogMessage "ExtPipeConfigs:create"
        |> HttpHandler.post item ConfigsUrl

    /// BETA: Get the current config revision
    let getCurrentConfig (extPipeId: string) (source: HttpHandler<unit>) : HttpHandler<ExtPipeConfig> =
        source
        |> withVersion ApiVersion.Playground
        |> withLogMessage "ExtPipeConfigs:getLatest"
        |> HttpHandler.get (ConfigsUrl +/ extPipeId)

    /// BETA: Get a specific config revision
    let getConfigRevision (extPipeId: string) (revision: int) (source: HttpHandler<unit>) : HttpHandler<ExtPipeConfig> =
        source
        |> withVersion ApiVersion.Playground
        |> withLogMessage "ExtPipeConfigs:getRevision"
        |> HttpHandler.getWithQuery (GetConfigQuery(Revision = revision)) (ConfigsUrl +/ extPipeId)

    /// BETA: List config revisions without details
    let listConfigRevisions (extPipeId: string) (query: ListConfigQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<ExtPipeConfig>> =
        source
        |> withVersion ApiVersion.Playground
        |> withLogMessage "ExtPipeConfigs:listRevisions"
        |> HttpHandler.getWithQuery query (ConfigsUrl +/ "revisions" +/ extPipeId)

    /// BETA: Revert to a previous config revision
    let revertConfigRevision (extPipeId: string) (revision: int) (source: HttpHandler<unit>) : HttpHandler<ExtPipeConfig> =
        source
        |> withVersion ApiVersion.Playground
        |> withLogMessage "ExtPipeConfigs:revert"
        |> HttpHandler.post (RevertConfigRequest(ExternalId = extPipeId, Revision = revision)) (ConfigsUrl +/ "revert")
