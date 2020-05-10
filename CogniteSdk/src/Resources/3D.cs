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
    public class ThreeDResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal ThreeDResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of ThreeDModels matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of ThreeDs matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<ThreeDModel>> ListAsync(ThreeDModelQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDs.list<ItemsWithCursor<ThreeDModel>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create ThreeDs.
        /// </summary>
        /// <param name="ThreeDs">ThreeDs to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created ThreeDs.</returns>
        public async Task<IEnumerable<ThreeDModel>> CreateAsync(IEnumerable<ThreeDModelCreate> ThreeDs, CancellationToken token = default)
        {
            if (ThreeDs is null)
            {
                throw new ArgumentNullException(nameof(ThreeDs));
            }

            var req = ThreeDs.create<IEnumerable<ThreeDModel>>(ThreeDs);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about an ThreeD given an ThreeD id.
        /// </summary>
        /// <param name="ThreeDId">The id of the ThreeD to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>ThreeD with the given id.</returns>
        public async Task<ThreeD> GetAsync(long ThreeDId, CancellationToken token = default)
        {
            var req = ThreeDs.get<ThreeD>(ThreeDId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple ThreeDs in the same project, along with all their descendants in the ThreeD hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="query">The query of ThreeDs to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(ThreeDDelete query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDs.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple ThreeDs in the same project by identity items.
        /// </summary>
        /// <param name="items">The list of ThreeDs identities to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> items, CancellationToken token = default)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var query = new ThreeDDelete() { Items = items };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple ThreeDs in the same project by internal ids.
        /// </summary>
        /// <param name="internalIds">The list of ThreeDs ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var query = new ThreeDDelete() { Items = internalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple ThreeDs in the same project by external ids.
        /// </summary>
        /// <param name="externalIds">The list of ThreeDs ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var query = new ThreeDDelete() { Items = externalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }
        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple ThreeDs in the same project. A maximum of 1000 ThreeDs IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of ThreeDs identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<ThreeD>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = ThreeDs.retrieve<IEnumerable<ThreeD>>(ids);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple ThreeDs in the same project. A maximum of 1000 ThreeDs IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of ThreeDs internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<ThreeD>> RetrieveAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple ThreeDs in the same project. A maximum of 1000 ThreeDs IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of ThreeDs internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<ThreeD>> RetrieveAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Retrieves a list of ThreeDs matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of ThreeDs matching given criteria.</returns>
        public async Task<IEnumerable<ThreeD>> SearchAsync (ThreeDSearch query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDs.search<IEnumerable<ThreeD>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Update one or more ThreeDs. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="query">The list of ThreeDs to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated ThreeDs.</returns>
        public async Task<IEnumerable<ThreeD>> UpdateAsync (IEnumerable<ThreeDUpdateItem> query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDs.update<IEnumerable<ThreeD>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}