// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various function HTTP handlers.
[<RequireQualifiedAccess>]
module Functions =
    [<Literal>]
    let Url = "/functions"

    let activate (source: HttpHandler<unit>) : HttpHandler<FunctionsActivationResponse> =
        source |> withLogMessage "Functions:activate" |> postV10 () (Url +/ "status")

    /// <summary>
    /// Retrieves list of functions.
    /// </summary>
    /// <returns>List of Functions.</returns>
    let list (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<Function>> =
        source |> withLogMessage "Functions:list" |> getV10 Url

    let filter (filter: FunctionsFilter) (limit: int) (source: HttpHandler<unit>) : HttpHandler<Function seq> =
        let request = FunctionsFilterRequest(Filter = filter, Limit = limit)

        http {
            let url = Url +/ "list"

            let! ret =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<FunctionsFilterRequest, ItemsWithoutCursor<Function>> request url

            return ret.Items
        }

    /// <summary>
    /// Create new Functions in the given project.
    /// </summary>
    /// <param name="functions">The Functions to create.</param>
    /// <returns>List of created Functions.</returns>
    let create (functions: FunctionCreate seq) (source: HttpHandler<unit>) : HttpHandler<Function seq> =
        source |> withLogMessage "Functions:create" |> create functions Url

    /// <summary>
    /// Delete multiple Functions in the same project.
    /// </summary>
    /// <param name="ids">The list of ids for Functions to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (ids: Identity seq) (ignoreUnknownIds: bool) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let items =
            ItemsWithIgnoreUnknownIds(Items = ids, IgnoreUnknownIds = ignoreUnknownIds)

        source |> withLogMessage "Functions:delete" |> delete items Url

    /// <summary>
    /// Retrieves information about multiple Functions in the same project. A maximum of 1000 Functions IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the Functions to get.</param>
    /// <returns>Functions with given ids.</returns>
    let retrieve (ids: Identity seq) (source: HttpHandler<unit>) : HttpHandler<Function seq> =
        source |> withLogMessage "Functions:retrieve" |> retrieve ids Url

    let getLimits (source: HttpHandler<unit>) : HttpHandler<FunctionLimits> =
        source |> withLogMessage "Functions:limits" |> getV10 (Url +/ "limits")

[<RequireQualifiedAccess>]
module FunctionCalls =
    [<Literal>]
    let Url = "/functions"

    /// Retrieves information about a function call given an functionId and a callId.
    let get (functionId: int64) (callId: int64) (source: HttpHandler<unit>) : HttpHandler<FunctionCall> =
        let url = Url +/ sprintf "%d" functionId +/ "calls"

        source |> withLogMessage "FunctionCalls:get" |> getById callId url

    /// <summary>
    /// Retrieves list of functionCalls matching filter.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <returns>List of FunctionCalls.</returns>
    let list
        (functionId: int64)
        (filter: FunctionCallFilter)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<FunctionCall>> =
        let url = Url +/ sprintf "%d" functionId +/ "calls"
        let filterQuery = FunctionCallQuery(Filter = filter)

        source |> withLogMessage "FunctionCalls:list" |> list filterQuery url

    /// <summary>
    /// Retrieves list of logs from function call.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <param name="callId">Id for function call to get.</param>
    /// <returns>List of logs from function call.</returns>
    let listLogs
        (functionId: int64)
        (callId: int64)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<FunctionCallLogEntry>> =
        let url = Url +/ sprintf "%d" functionId +/ "calls" +/ sprintf "%d" callId +/ "logs"

        source |> withLogMessage "FunctionCalls:listLogs" |> getV10 url

    /// <summary>
    /// Retrieves response for function call.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <param name="callId">Id for function call to get.</param>
    /// <returns>Response from function call.</returns>
    let retrieveResponse
        (functionId: int64)
        (callId: int64)
        (source: HttpHandler<unit>)
        : HttpHandler<FunctionCallResponse> =
        let url =
            Url +/ sprintf "%d" functionId +/ "calls" +/ sprintf "%d" callId +/ "response"

        source |> withLogMessage "FunctionCalls:retrieveResponse" |> getV10 url

    /// <summary>
    /// Call a function synchronously.
    /// </summary>
    /// <param name="functionId">Id for function to get call from.</param>
    /// <param name="data">Data passed through the data argument to the function.</param>
    /// <returns>Function call response.</returns>
    let callFunction
        (functionId: int64)
        (data: 'b)
        (nonce: string)
        (source: HttpHandler<unit>)
        : HttpHandler<FunctionCall> =
        let url = Url +/ sprintf "%d" functionId +/ "call"
        let dataDto = FunctionCallData(Data = data, Nonce = nonce)

        source |> withLogMessage "FunctionCalls:CallFunction" |> postV10 dataDto url

[<RequireQualifiedAccess>]
module FunctionSchedules =
    [<Literal>]
    let Url = "/functions/schedules"

    /// <summary>
    /// Retrieves list of functionSchedules.
    /// </summary>
    /// <returns>List of FunctionSchedules.</returns>
    let list (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<FunctionSchedule>> =
        source |> withLogMessage "FunctionSchedules:list" |> getV10 Url

    /// <summary>
    /// Create new FunctionSchedules in the given project.
    /// </summary>
    /// <param name="functionSchedules">The FunctionSchedules to create.</param>
    /// <returns>List of created FunctionSchedules.</returns>
    let create
        (functionSchedules: FunctionScheduleCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<FunctionSchedule seq> =
        source
        |> withLogMessage "FunctionSchedules:create"
        |> create functionSchedules Url

    /// <summary>
    /// Delete multiple FunctionSchedules in the same project.
    /// </summary>
    /// <param name="ids">The list of server-ids for FunctionSchedules to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (ids: int64 seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let items = ItemsWithoutCursor(Items = ids)

        source |> withLogMessage "FunctionSchedules:delete" |> delete items Url
