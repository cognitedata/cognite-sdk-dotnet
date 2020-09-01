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
        /// <param name="includeMetadata">Include meta-data in responses or not.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal AssetsResource(Func<CancellationToken, Task<string>> authHandler, bool includeMetadata, HttpContext ctx) : base(authHandler, includeMetadata, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<IItemsWithCursor<Asset>> ListAsync(AssetQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (_includeMetadata)
            {
                var req = Assets.list<Asset, ItemsWithCursor<Asset>>(query);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Assets.list<AssetWithoutMetadata, ItemsWithCursor<AssetWithoutMetadata>>(query);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves list of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public IItemsWithCursor<Asset> List(AssetQuery query, CancellationToken token = default)
        {
            return ListAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves number of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of assets matching given filters and optional cursor</returns>
        public async Task<int> AggregateAsync(AssetQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.aggregate<int>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves number of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of assets matching given filters and optional cursor</returns>
        public int Aggregate(AssetQuery query, CancellationToken token = default)
        {
            return AggregateAsync(query, token).GetAwaiter().GetResult();
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
        /// Create assets.
        /// </summary>
        /// <param name="assets">Assets to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created assets.</returns>
        public IEnumerable<Asset> Create(IEnumerable<AssetCreate> assets, CancellationToken token = default)
        {
            return CreateAsync(assets, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public async Task<Asset> GetAsync(long assetId, CancellationToken token = default)
        {
            if (_includeMetadata)
            {
                var req = Assets.get<Asset, Asset>(assetId);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Assets.get<AssetWithoutMetadata, AssetWithoutMetadata>(assetId);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public Asset Get(long assetId, CancellationToken token = default)
        {
            return GetAsync(assetId, token).GetAwaiter().GetResult();
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
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="query">The query of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(AssetDelete query, CancellationToken token = default)
        {
            return DeleteAsync(query, token).GetAwaiter().GetResult();
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
        /// Delete multiple assets in the same project by identity items.
        /// </summary>
        /// <param name="items">The list of assets identities to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<Identity> items, CancellationToken token = default)
        {
            return DeleteAsync(items, token).GetAwaiter().GetResult();
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
        /// Delete multiple assets in the same project by internal ids.
        /// </summary>
        /// <param name="internalIds">The list of assets ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            return DeleteAsync(internalIds, token).GetAwaiter().GetResult();
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

        /// <summary>
        /// Delete multiple assets in the same project by external ids.
        /// </summary>
        /// <param name="externalIds">The list of assets ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            return DeleteAsync(externalIds, token).GetAwaiter().GetResult();
        }
        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of assets identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            if (_includeMetadata)
            {
                var req = Assets.retrieve<Asset, IEnumerable<Asset>>(ids, ignoreUnknownIds);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Assets.retrieve<AssetWithoutMetadata, IEnumerable<AssetWithoutMetadata>>(ids, ignoreUnknownIds);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of assets identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<Asset> Retrieve(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(ids, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<Asset> Retrieve(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(internalIds, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Asset>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<Asset> Retrieve(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(externalIds, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }
        #endregion

        /// <summary>
        /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<Asset>> SearchAsync(AssetSearch query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (_includeMetadata)
            {
                var req = Assets.search<Asset, IEnumerable<Asset>>(query);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
            else
            {
                var req = Assets.search<AssetWithoutMetadata, IEnumerable<AssetWithoutMetadata>>(query);
                return await RunAsync(req, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public IEnumerable<Asset> Search(AssetSearch query, CancellationToken token = default)
        {
            return SearchAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<IEnumerable<Asset>> UpdateAsync(IEnumerable<AssetUpdateItem> query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Assets.update<Asset, IEnumerable<Asset>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public IEnumerable<Asset> Update(IEnumerable<AssetUpdateItem> query, CancellationToken token = default)
        {
            return UpdateAsync(query, token).GetAwaiter().GetResult();
        }
    }
}