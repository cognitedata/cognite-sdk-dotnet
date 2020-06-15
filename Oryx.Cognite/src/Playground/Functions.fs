// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Playground

open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Playground

open System.Collections.Generic
open CogniteSdk
open CogniteSdk.Types.Common
open System.Text.Json

/// Various function HTTP handlers.
[<RequireQualifiedAccess>]
module Functions =
    [<Literal>]
    let Url = "/functions"

    /// <summary>
    /// Retrieves list of functions.
    /// </summary>
    /// <returns>List of Functions.</returns>
    let list () : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<Function>, 'a> =
        withLogMessage "Functions:list"
        >=> getPlayground Url

    /// <summary>
    /// Create new Functions in the given project.
    /// </summary>
    /// <param name="functions">The Functions to create.</param>
    /// <returns>List of created Functions.</returns>
    let create (functions: FunctionCreate seq) : HttpHandler<HttpResponseMessage, Function seq, 'a> =
        withLogMessage "Functions:create"
        >=> create functions Url

    /// <summary>
    /// Delete multiple Functions in the same project.
    /// </summary>
    /// <param name="ids">The list of ids for Functions to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (ids: Identity seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let items = ItemsWithoutCursor(Items=ids)
        withLogMessage "Functions:delete"
        >=> delete items Url

    /// <summary>
    /// Retrieves information about multiple Functions in the same project. A maximum of 1000 Functions IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the Functions to get.</param>
    /// <returns>Functions with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, Function seq, 'a> =
        withLogMessage "Functions:retrieve"
        >=> retrieve ids Url

[<RequireQualifiedAccess>]
module FunctionCalls =
    [<Literal>]
    let Url = "/functions"

    /// Retrieves information about a function call given an functionId and a callId.
    let get (functionId: int64) (callId: int64) : HttpHandler<HttpResponseMessage, FunctionCall, 'a> =
        let url = Url +/ sprintf "%d" functionId +/ "calls"
        withLogMessage "FunctionCalls:get"
        >=> getById callId Url

    /// <summary>
    /// Retrieves list of functionCalls matching filter.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <returns>List of FunctionCalls.</returns>
    let list (functionId: int64) (filter: FunctionCallFilter): HttpHandler<HttpResponseMessage, ItemsWithoutCursor<FunctionCall>, 'a> =
        let url = Url +/ sprintf "%d" functionId +/ "calls"
        withLogMessage "FunctionCalls:list"
        >=> list filter url

    /// <summary>
    /// Retrieves list of logs from function call.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <param name="callId">Id for function call to get.</param>
    /// <returns>List of logs from function call.</returns>
    let listLogs (functionId: int64) (callId: int64) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<FunctionCallLogEntry>, 'a> =
        let url = Url +/ sprintf "%d" functionId +/ "calls" +/ sprintf "%d" callId +/ "logs"
        withLogMessage "FunctionCalls:list"
        >=> getPlayground url

    /// <summary>
    /// Retrieves response for function call.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <param name="callId">Id for function call to get.</param>
    /// <returns>Response from function call.</returns>
    let retrieveResponse (functionId: int64) (callId: int64) : HttpHandler<HttpResponseMessage, FunctionCallResponse, 'a> =
        let url = Url +/ sprintf "%d" functionId +/ "calls" +/ sprintf "%d" callId +/ "response"
        withLogMessage "FunctionCalls:retrieveResponse"
        >=> getPlayground url

    /// <summary>
    /// Call a function synchronously.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <param name="data">Data passed through the data argument to the function.</param>
    /// <returns>Function call response.</returns>
    let callFunction (functionId: int64) (data: JsonElement): HttpHandler<HttpResponseMessage, FunctionCall, 'a> =
        let url = Url +/ sprintf "%d" functionId +/ "call"
        withLogMessage "FunctionCalls:CallFunction"
        >=> postPlayground data url

[<RequireQualifiedAccess>]
module FunctionSchedules =
    [<Literal>]
    let Url = "/functions/schedules"

    /// <summary>
    /// Retrieves list of functionSchedules.
    /// </summary>
    /// <returns>List of FunctionSchedules.</returns>
    let list () : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<FunctionSchedule>, 'a> =
        withLogMessage "FunctionSchedules:list"
        >=> getPlayground Url

    /// <summary>
    /// Create new FunctionSchedules in the given project.
    /// </summary>
    /// <param name="functionSchedules">The FunctionSchedules to create.</param>
    /// <returns>List of created FunctionSchedules.</returns>
    let create (functionSchedules: FunctionScheduleCreate seq) : HttpHandler<HttpResponseMessage, FunctionSchedule seq, 'a> =
        withLogMessage "FunctionSchedules:create"
        >=> create functionSchedules Url

    /// <summary>
    /// Delete multiple FunctionSchedules in the same project.
    /// </summary>
    /// <param name="ids">The list of server-ids for FunctionSchedules to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (ids: CogniteServerId seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let items = ItemsWithoutCursor(Items=ids)
        withLogMessage "FunctionSchedules:delete"
        >=> delete items Url
