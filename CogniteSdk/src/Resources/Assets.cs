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
    /// For internal use. Contains all asset methods.
    /// </summary>
    public class AssetsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal AssetsResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<Asset>> ListAsync(AssetQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.list<ItemsWithCursor<Asset>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves number of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of assets matching given filters and optional cursor</returns>
        public async Task<Int32> CountAsync(AssetQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.count<Int32>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create assets.
        /// </summary>
        /// <param name="assets">Assets to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created assets.</returns>
        public async Task<IEnumerable<Asset>> CreateAsync(IEnumerable<AssetCreate> assets, CancellationToken token = default)
        {
            if (assets is null)
            {
                throw new ArgumentNullException(nameof(assets));
            }

            var req = Assets.create<IEnumerable<Asset>>(assets);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public async Task<Asset> GetAsync(long assetId, CancellationToken token = default)
        {
            var req = Assets.get<Asset>(assetId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="query">The query of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(AssetDelete query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple assets in the same project by identity items.
        /// </summary>
        /// <param name="items">The list of assets identities to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> items, CancellationToken token = default)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var query = new AssetDelete() { Items = items };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple assets in the same project by internal ids.
        /// </summary>
        /// <param name="internalIds">The list of assets ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var query = new AssetDelete() { Items = internalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple assets in the same project by external ids.
        /// </summary>
        /// <param name="externalIds">The list of assets ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var query = new AssetDelete() { Items = externalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }
        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of assets identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Assets.retrieve<IEnumerable<Asset>>(ids);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<string> externalIds, CancellationToken token = default)
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
        /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<Asset>> SearchAsync (AssetSearch query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.search<IEnumerable<Asset>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<IEnumerable<Asset>> UpdateAsync (IEnumerable<AssetUpdateItem> query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.update<IEnumerable<Asset>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}