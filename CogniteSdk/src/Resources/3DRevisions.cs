// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Oryx.Cognite;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all ThreeD methods.
    /// </summary>
    public class ThreeDRevisionsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal ThreeDRevisionsResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of ThreeDRevisions matching query.
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of ThreeDRevision matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<ThreeDRevision>> ListAsync(string modelId, ThreeDRevisionQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDRevisions.list<ItemsWithCursor<ThreeDRevision>>(modelId, query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of ThreeDRevisions matching query.
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="revisionId">The 3D revision to get logs from.</param>
        /// <param name="severity">Filter on severity of logs.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of ThreeDRevision matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<ThreeDRevisionLog>> ListLogsAsync(string modelId, string revisionId, long severity, CancellationToken token = default)
        {
            var req = ThreeDRevisions.listLogs<ItemsWithCursor<ThreeDRevisionLog>>(modelId, revisionId, severity);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of ThreeDRevisions matching query.
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="revisionId">The 3D revision to get nodes from.</param>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of ThreeDRevision matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<ThreeDNode>> ListNodesAsync(string modelId, string revisionId, ThreeDNodeQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDNodes.list<ItemsWithCursor<ThreeDNode>>(modelId, revisionId, query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create ThreeDRevision.
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="ThreeDRevision">ThreeDRevision to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created ThreeDRevision.</returns>
        public async Task<IEnumerable<ThreeDRevision>> CreateAsync(string modelId, IEnumerable<ThreeDRevisionCreate> ThreeDRevision, CancellationToken token = default)
        {
            if (ThreeDRevision is null)
            {
                throw new ArgumentNullException(nameof(ThreeDRevision));
            }

            var req = ThreeDRevisions.create<IEnumerable<ThreeDRevision>>(modelId, ThreeDRevision);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple ThreeDRevision in the same project, along with all their descendants in the ThreeD hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="ids">Ids of ThreeDRevisions to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(string modelId, IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }


            var req = ThreeDRevisions.delete<EmptyResponse>(modelId, ids);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple ThreeDRevision in the same project by internal ids.
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="internalIds">The list of ThreeDRevision ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(string modelId, IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var query = internalIds.Select(Identity.Create);
            return await DeleteAsync(modelId, query, token).ConfigureAwait(false);
        }

        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple ThreeDRevision in the same project. A maximum of 1000 ThreeDRevision IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="revisionId">The 3D revision to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<ThreeDRevision>> RetrieveAsync(string modelId, long revisionId, CancellationToken token = default)
        {
            var req = ThreeDRevisions.retrieve<IEnumerable<ThreeDRevision>>(modelId, revisionId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #endregion

        /// <summary>
        /// Update one or more ThreeDRevision. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="query">The list of ThreeDRevision to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated ThreeDRevision.</returns>
        public async Task<IEnumerable<ThreeDRevision>> UpdateAsync (string modelId, IEnumerable<ThreeDRevisionUpdateItem> query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDRevisions.update<IEnumerable<ThreeDRevision>>(modelId, query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Update one or more ThreeDRevision. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="modelId">The 3D model to get revision from.</param>
        /// <param name="revisionId">The 3D revision to get file from.</param>
        /// <param name="fileId">The 3D file to update thumbnail on.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated ThreeDRevision.</returns>
        public async Task<EmptyResponse> UpdateThumbnailAsync (string modelId, string revisionId, long fileId, CancellationToken token = default )
        {
            var req = ThreeDRevisions.updateThumbnail<EmptyResponse>(modelId, revisionId, fileId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}