// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Relationships;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all relationship methods.
    /// </summary>
    public class RelationshipResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal RelationshipResource(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of relationships matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of Relationships matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<RelationshipRead>> ListAsync(RelationshipQueryDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Relationships.list<ItemsWithCursor<RelationshipRead>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create Relationships.
        /// </summary>
        /// <param name="relationships">Relationships to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created Relationships.</returns>
        public async Task<ItemsWithoutCursor<RelationshipRead>> CreateAsync(IEnumerable<RelationshipWriteDto> relationships, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Relationships.create<ItemsWithoutCursor<RelationshipRead>>(relationships);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple relationships in the same project, along with all their descendants in the relationship hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="externalIds">The externalIds of relationships to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Relationships.delete<EmptyResponse>(externalIds);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple relationships in the same project. A maximum of 1000 relationships IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of relationships identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<ItemsWithoutCursor<RelationshipRead>> RetrieveAsync(IEnumerable<string> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Relationships.retrieve<ItemsWithoutCursor<RelationshipRead>>(ids);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of relationships matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of relationships matching given criteria.</returns>
        public async Task<RestrictedGraphQueryResult> SearchAsync (RestrictedGraphQuery query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.Relationships.search<RestrictedGraphQueryResult>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}