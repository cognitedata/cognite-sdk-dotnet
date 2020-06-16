// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk;
using Oryx.Cognite.Playground;
using static Oryx.Cognite.Playground.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources.Playground
{
    /// <summary>
    /// For internal use. Contains all Function call methods.
    /// </summary>
    public class FunctionCallResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal FunctionCallResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves information about a functionCall given a function id and a call id.
        /// </summary>
        /// <param name="functionId">The id of the function to get call from.</param>
        /// <param name="callId">The id of the function to get call from.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Function call with the given id.</returns>
        public async Task<FunctionCall> GetAsync(long functionId, long callId, CancellationToken token = default)
        {
            var req = FunctionCalls.get<FunctionCall>(functionId, callId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of Function calls matching query.
        /// </summary>
        /// <param name="functionId">The id of the function to get call from.</param>
        /// <param name="filter">Filter to find function calls.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of Functions</returns>
        public async Task<ItemsWithoutCursor<FunctionCall>> ListAsync(long functionId, FunctionCallFilter filter, CancellationToken token = default)
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            var req = FunctionCalls.list<ItemsWithoutCursor<FunctionCall>>(functionId, filter);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of Function call logs.
        /// </summary>
        /// <param name="functionId">Id for function to get call from.</param>
        /// <param name="callId">Id for function call to get.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of Functions</returns>
        public async Task<ItemsWithoutCursor<FunctionCallLogEntry>> ListLogsAsync(long functionId, long callId, CancellationToken token = default)
        {
            var req = FunctionCalls.listLogs<ItemsWithoutCursor<FunctionCallLogEntry>>(functionId, callId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves response for Function call.
        /// </summary>
        /// <param name="functionId">Id for function to get call from.</param>
        /// <param name="callId">Id for function call to get.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Reponse from function call.</returns>
        public async Task<FunctionCallResponse> RetrieveResponse(long functionId, long callId, CancellationToken token = default)
        {
            var req = FunctionCalls.retrieveResponse<FunctionCallResponse>(functionId, callId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls a function synchronously.
        /// </summary>
        /// <param name="functionId">Id for function to get call from.</param>
        /// <param name="data">Data passed through the data argument to the function.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Reponse from function call.</returns>
        public async Task<FunctionCall> CallFunction<T>(long functionId, T data, CancellationToken token = default)
        {
            var req = FunctionCalls.callFunction<T, FunctionCall>(functionId, data);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}