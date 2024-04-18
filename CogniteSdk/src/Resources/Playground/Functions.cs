// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;

using Oryx;
using Oryx.Cognite.Playground;

namespace CogniteSdk.Resources.Playground
{
    /// <summary>
    /// For internal use. Contains all Function methods.
    /// </summary>
    public class FunctionResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal FunctionResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of Functions matching query.
        /// </summary>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of Functions</returns>
        public async Task<ItemsWithoutCursor<Function>> ListAsync(CancellationToken token = default)
        {
            var req = Functions.list(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create Functions.
        /// </summary>
        /// <param name="functions">Functions to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created Functions.</returns>
        public async Task<IEnumerable<Function>> CreateAsync(IEnumerable<FunctionCreate> functions, CancellationToken token = default)
        {
            if (functions is null)
            {
                throw new ArgumentNullException(nameof(functions));
            }

            var req = Functions.create(functions, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple Functions in the same project, along with all their descendants in the Function hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="ids">The ids of Functions to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Functions.delete(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple Functions in the same project. A maximum of 1000 Functions IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of Functions identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Function>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Functions.retrieve(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}