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
    /// Contains all Function methods.
    /// </summary>
    public class FunctionResource : Resource
    {
        /// <summary>
        /// Contains method for function calls.
        /// </summary>
        public FunctionCallResource Calls { get; }

        /// <summary>
        /// Contains methods for function schedules.
        /// </summary>
        public FunctionScheduleResource Schedules { get; }


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
        /// <param name="ignoreUnknownIds">True to ignore functions that were not found.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> ids, bool ignoreUnknownIds, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Functions.delete(ids, ignoreUnknownIds, GetContext(token));
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

        /// <summary>
        /// Activate functions for the given project, or get activation status.
        /// </summary>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<FunctionsActivationResponse> ActivateAsync(CancellationToken token = default)
        {
            var req = Functions.activate(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Get limits for functions in the current project.
        /// </summary>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<FunctionLimits> GetLimitsAsync(CancellationToken token = default)
        {
            var req = Functions.getLimits(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}