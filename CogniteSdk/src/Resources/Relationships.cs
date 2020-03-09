// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all relationship methods.
    /// </summary>
    public class RelationshipResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal RelationshipResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of relationships matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of Relationships matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<Relationship>> ListAsync(RelationshipQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Relationships.list<ItemsWithCursor<Relationship>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create Relationships.
        /// </summary>
        /// <param name="relationships">Relationships to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created Relationships.</returns>
        public async Task<ItemsWithoutCursor<Relationship>> CreateAsync(IEnumerable<RelationshipCreate> relationships, CancellationToken token = default)
        {
            if (relationships is null)
            {
                throw new ArgumentNullException(nameof(relationships));
            }

            var req = Oryx.Cognite.Relationships.create<ItemsWithoutCursor<Relationship>>(relationships);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple relationships in the same project, along with all their descendants in the relationship hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="externalIds">The externalIds of relationships to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var req = Oryx.Cognite.Relationships.delete<EmptyResponse>(externalIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple relationships in the same project. A maximum of 1000 relationships IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of relationships identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<ItemsWithoutCursor<Relationship>> RetrieveAsync(IEnumerable<string> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Oryx.Cognite.Relationships.retrieve<ItemsWithoutCursor<Relationship>>(ids);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of relationships matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of relationships matching given criteria.</returns>
        public async Task<RestrictedGraphQueryResult> SearchAsync (RestrictedGraphQuery query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Relationships.search<RestrictedGraphQueryResult>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}