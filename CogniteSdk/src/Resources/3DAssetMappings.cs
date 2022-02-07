// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite;
using Oryx.Pipeline;
using static Oryx.Cognite.HttpHandlerModule;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all ThreeD methods.
    /// </summary>
    public class ThreeDAssetMappingsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal ThreeDAssetMappingsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of ThreeDAssetMappings matching query.
        /// </summary>
        /// <param name="modelId">The modelId to get revision from.</param>
        /// <param name="revisionId">The revisionId to get asset mappings from</param>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of ThreeDAssetMapping matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<ThreeDAssetMapping>> ListAsync(long modelId, long revisionId, ThreeDAssetMappingQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDAssetMappings.list(modelId, revisionId, query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create ThreeDAssetMapping.
        /// </summary>
        /// <param name="modelId">The modelId to get revision from.</param>
        /// <param name="revisionId">The revisionId to get asset mappings from</param>
        /// <param name="ThreeDAssetMapping">ThreeDAssetMapping to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created ThreeDAssetMapping.</returns>
        public async Task<IEnumerable<ThreeDAssetMapping>> CreateAsync(long modelId, long revisionId, IEnumerable<ThreeDAssetMappingCreate> ThreeDAssetMapping, CancellationToken token = default)
        {
            if (ThreeDAssetMapping is null)
            {
                throw new ArgumentNullException(nameof(ThreeDAssetMapping));
            }

            var req = ThreeDAssetMappings.create(modelId, revisionId, ThreeDAssetMapping, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple ThreeDAssetMapping in the same project, along with all their descendants in the ThreeD hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="modelId">The modelId to get revision from.</param>
        /// <param name="revisionId">The revisionId to get asset mappings from</param>
        /// <param name="ids">Ids of ThreeDAssetMappings to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(long modelId, long revisionId, IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = ThreeDAssetMappings.delete(modelId, revisionId, ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple ThreeDAssetMapping in the same project by internal ids.
        /// </summary>
        /// <param name="modelId">The modelId to get revision from.</param>
        /// <param name="revisionId">The revisionId to get asset mappings from</param>
        /// <param name="internalIds">The list of ThreeDAssetMapping ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(long modelId, long revisionId, IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var query = internalIds.Select(Identity.Create);
            return await DeleteAsync(modelId, revisionId, query, token).ConfigureAwait(false);
        }
        #endregion
    }
}
