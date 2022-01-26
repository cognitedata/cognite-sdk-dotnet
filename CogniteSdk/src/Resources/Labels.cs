// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Labels resource
    /// </summary>
    public class LabelsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal LabelsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Unit, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Exception, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, Task<Unit>>, Task<Unit>>>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// List labels with pagination and optional filters.
        /// </summary>
        /// <param name="query">Label query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of retrieved labels with cursor if the limit was reached.</returns>
        public async Task<ItemsWithCursor<Label>> ListAsync(LabelQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Labels.list(query, _ctx);
            return await RunAsync(req, token);
        }

        /// <summary>
        /// Create label definitions.
        /// </summary>
        /// <param name="labels">List of label definitions to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of created labels</returns>
        public async Task<IEnumerable<Label>> CreateAsync(IEnumerable<LabelCreate> labels, CancellationToken token = default)
        {
            if (labels is null) throw new ArgumentNullException(nameof(labels));

            var req = Labels.create(labels, _ctx);
            return await RunAsync(req, token);
        }

        #region Delete overloads
        /// <summary>
        /// Delete label definitions.
        /// </summary>
        /// <param name="query">Delete query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(LabelDelete query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Labels.delete(query, _ctx);
            await RunAsync(req, token).ConfigureAwait(false);
        }


        /// <summary>
        /// Delete label definitions.
        /// </summary>
        /// <param name="ids">External ids to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(IEnumerable<CogniteExternalId> ids, CancellationToken token = default)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));

            var req = new LabelDelete
            {
                Items = ids
            };
            await DeleteAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete label definitions.
        /// </summary>
        /// <param name="externalIds">External ids to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            if (externalIds is null) throw new ArgumentNullException(nameof(externalIds));

            var req = new LabelDelete
            {
                Items = externalIds.Select(id => new CogniteExternalId(id))
            };
            await DeleteAsync(req, token).ConfigureAwait(false);
        }
        #endregion
    }
}
