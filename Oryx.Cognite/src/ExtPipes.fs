// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic

open Oryx
open Oryx.Cognite

open CogniteSdk
open Oryx.SystemTextJson
open Oryx.SystemTextJson.ResponseReader
open System.Net.Http
open System.Text.Json
open FSharp.Control.Tasks

/// Various extraction pipeline HTTP handlers
[<RequireQualifiedAccess>]
module ExtPipes = 
    [<Literal>]
    let Url = "/extpipes"
    let RunsUrl = Url +/ "runs"

    /// Retrieves list of extraction pipelines matching filter, and a cursor if given limit is exceeded.
    let list (query: ExtPipeQuery) : IHttpHandler<unit, ItemsWithCursor<ExtPipe>> = 
        withLogMessage "ExtPipes:get"
        >=> list query Url

    /// Create new extraction pipelines in the given project. Returns list of created extraction pipelines.
    let create (items: ExtPipeCreate seq) : IHttpHandler<unit, ExtPipe seq> =
        withLogMessage "ExtPipes:create"
        >=> create items Url

    /// Delete multiple extraction pipelines.
    /// Returns an empty response, not JSON.
    let delete (items: ExtPipeDelete) : IHttpHandler<unit, unit> =
        let url = Url +/ "delete"
        POST
        >=> withVersion V10
        >=> withLogMessage "ExtPipes:delete"
        >=> withContent (fun () -> new JsonPushStreamContent<ExtPipeDelete>(items, jsonOptions) :> _)
        >=> withResource url
        >=> fetch
        >=> withError decodeError
        >=> skip
        >=> log

    /// Update extraction pipelines
    let update (query: IEnumerable<UpdateItem<ExtPipeUpdate>>) : IHttpHandler<unit, ExtPipe seq> =
        withLogMessage "ExtPipes:update"
        >=> update query Url

    /// Retreive a list of extraction pipelines. Optionally ignore unknown ids.
    let retrieve (ids: Identity seq) (ignoreUnknownIds: Nullable<bool>) : IHttpHandler<unit, ExtPipe seq> =
        withLogMessage "ExtPipes:retrieve"
        >=> retrieveIgnoreUnkownIds ids (Option.ofNullable ignoreUnknownIds) Url

    /// Retrieves list of extraction pipeline runs matching filter, and a cursor if given limit is exceeded.
    let listRuns (query: ExtPipeRunQuery) : IHttpHandler<unit, ItemsWithCursor<ExtPipeRun>> =
        withLogMessage "ExtPipeRuns:get"
        >=> HttpHandler.list query RunsUrl

    /// Create new extraction pipeline runs in the given project. Returns list of created extraction pipeline runs.
    let createRuns (items: ExtPipeRunCreate seq) : IHttpHandler<unit, ExtPipeRun seq> =
        withLogMessage "ExtPipeRuns:create"
        >=> HttpHandler.create items RunsUrl