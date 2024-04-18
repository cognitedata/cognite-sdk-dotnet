// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite.Playground;

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
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal FunctionCallResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
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
            var req = FunctionCalls.get(functionId, callId, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
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
            var req = FunctionCalls.list(functionId, filter, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
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
            var req = FunctionCalls.listLogs(functionId, callId, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves response for Function call.
        /// </summary>
        /// <param name="functionId">Id for function to get call from.</param>
        /// <param name="callId">Id for function call to get.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Response from function call.</returns>
        public async Task<FunctionCallResponse> RetrieveResponse(long functionId, long callId, CancellationToken token = default)
        {
            var req = FunctionCalls.retrieveResponse(functionId, callId, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls a function synchronously.
        /// </summary>
        /// <param name="functionId">Id for function to get call from.</param>
        /// <param name="data">Data passed through the data argument to the function.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Response from function call.</returns>
        public async Task<FunctionCall> CallFunction<T>(long functionId, T data, CancellationToken token = default)
        {
            var req = FunctionCalls.callFunction<T>(functionId, data, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}