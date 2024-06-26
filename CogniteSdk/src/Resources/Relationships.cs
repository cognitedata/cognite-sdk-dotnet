// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources
{
    using CogniteSdk;

    /// <summary>
    /// For internal use. Contains all relationship methods.
    /// </summary>
    public class RelationshipResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal RelationshipResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
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

            var req = Oryx.Cognite.Relationships.list(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create Relationships.
        /// </summary>
        /// <param name="relationships">Relationships to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created Relationships.</returns>
        public async Task<IEnumerable<Relationship>> CreateAsync(IEnumerable<RelationshipCreate> relationships, CancellationToken token = default)
        {
            if (relationships is null)
            {
                throw new ArgumentNullException(nameof(relationships));
            }

            var req = Oryx.Cognite.Relationships.create(relationships, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple relationships in the same project, along with all their descendants in the relationship
        /// hierarchy if recursive is true.
        /// </summary>
        /// <param name="externalIds">The externalIds of relationships to delete.</param>
        /// <param name="ignoreUnknownIds">If true, ignore any unknown externalIds in the request</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, bool ignoreUnknownIds = false, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var req = Oryx.Cognite.Relationships.delete(externalIds, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple relationships in the same project. A maximum of 1000 relationships IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of relationships identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">If true, ignore any unknown externalIds in the request</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Relationship>> RetrieveAsync(IEnumerable<string> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Oryx.Cognite.Relationships.retrieve(ids, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates multiple relationships within the same project. This operation supports partial updates, meaning that
        /// fields omitted from the requests are not changed. Returns list of updated relationships.
        /// </summary>
        /// <param name="items">List of update items</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns> Updated relationships.</returns>
        public async Task<IEnumerable<Relationship>> UpdateAsync(IEnumerable<UpdateItem<RelationshipUpdate>> items, CancellationToken token = default)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var req = Oryx.Cognite.Relationships.update(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
