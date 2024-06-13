// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;

using Oryx;
using Oryx.Cognite;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all FunctionSchedule methods.
    /// </summary>
    public class FunctionScheduleResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal FunctionScheduleResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of FunctionSchedules matching query.
        /// </summary>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of FunctionSchedules</returns>
        public async Task<ItemsWithoutCursor<FunctionSchedule>> ListAsync(CancellationToken token = default)
        {
            var req = FunctionSchedules.list(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create FunctionSchedules.
        /// </summary>
        /// <param name="functionSchedules">FunctionSchedules to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created FunctionSchedules.</returns>
        public async Task<IEnumerable<FunctionSchedule>> CreateAsync(IEnumerable<FunctionScheduleCreate> functionSchedules, CancellationToken token = default)
        {
            if (functionSchedules is null)
            {
                throw new ArgumentNullException(nameof(functionSchedules));
            }

            var req = FunctionSchedules.create(functionSchedules, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple FunctionSchedules in the same project, along with all their descendants in the Function hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="ids">The ids of FunctionSchedules to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = FunctionSchedules.delete(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}