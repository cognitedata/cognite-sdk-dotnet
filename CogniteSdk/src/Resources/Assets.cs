// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Oryx;
using Oryx.Cognite;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all asset methods.
    /// </summary>
    public class AssetsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal AssetsResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Asynchronously retrieve a list of asset like objects matching the given query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <typeparam name="T">Type of asset to return, e.g Assset or AssetWithoutMetadata.</typeparam>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<IItemsWithCursor<T>> ListAsync<T>(AssetQuery query, CancellationToken token = default) where T : Asset
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.list<T>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<Asset>> ListAsync(AssetQuery query, CancellationToken token = default)
        {
            return (ItemsWithCursor<Asset>) await ListAsync<Asset>(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve number of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of assets matching given filters</returns>
        public async Task<int> AggregateAsync(AssetQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.aggregate(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously create assets.
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

            var req = Assets.create(assets);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about an asset like object given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <typeparam name="T">Type of asset to return, e.g Assset or AssetWithoutMetadata.</typeparam>
        /// <returns>Asset with the given id.</returns>
        public async Task<T> GetAsync<T>(long assetId, CancellationToken token = default) where T : Asset
        {
            var req = Assets.get<T>(assetId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about an asset given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public async Task<Asset> GetAsync(long assetId, CancellationToken token = default)
        {
            return await GetAsync<Asset>(assetId, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Asynchronously delete multiple assets in the same project, along with all their descendants in the asset
        /// hierarchy if recursive is true.
        /// </summary>
        /// <param name="query">The query of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(AssetDelete query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.delete(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously delete multiple assets in the same project by identity items.
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
        /// Asynchronously delete multiple assets in the same project by internal ids.
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
        /// Asynchronously delete multiple assets in the same project by external ids.
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
        /// Asynchronously retrieves information about multiple asset like objects in the same project. A maximum of
        /// 1000 assets IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of assets identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <typeparam name="T">Type of asset to return, e.g Assset or AssetWithoutMetadata.</typeparam>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested assets.</returns>
        public async Task<IEnumerable<T>> RetrieveAsync<T>(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : Asset
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Assets.retrieve<T>(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of assets identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested assets.</returns>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return await RetrieveAsync<Asset>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested assets.</returns>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync<Asset>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested assets.</returns>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync<Asset>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        #endregion

        /// <summary>
        /// Asynchronously retrieve a list of asset like objects matching the given criteria. This operation does not
        /// support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <typeparam name="T">Type of asset to return, e.g Assset or AssetWithoutMetadata.</typeparam>
        /// <returns>Sequence of assets matching given criteria.</returns>
        public async Task<IEnumerable<T>> SearchAsync<T>(AssetSearch query, CancellationToken token = default) where T : Asset
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.search<T>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of assets matching the given criteria. This operation does not support
        /// pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of assets matching given criteria.</returns>
        public async Task<IEnumerable<Asset>> SearchAsync(AssetSearch query, CancellationToken token = default)
        {
            return await SearchAsync<Asset>(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously update one or more assets. Supports partial updates, meaning that fields omitted from the
        /// requests are not changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of updated assets.</returns>
        public async Task<IEnumerable<Asset>> UpdateAsync(IEnumerable<AssetUpdateItem> query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.update<Asset>(query);
            var ret = await RunAsync<IEnumerable<Asset>>(req, token).ConfigureAwait(false);
            return ret;
        }
    }
}