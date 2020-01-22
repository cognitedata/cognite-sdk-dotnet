// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Assets;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all asset methods.
    /// </summary>
    public class AssetsResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal AssetsResource(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<AssetReadDto>> ListAsync(AssetQueryDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Assets.list<ItemsWithCursor<AssetReadDto>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create assets.
        /// </summary>
        /// <param name="assets">Assets to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created assets.</returns>
        public async Task<IEnumerable<AssetReadDto>> CreateAsync(IEnumerable<AssetWriteDto> assets, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Assets.create<IEnumerable<AssetReadDto>>(assets);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public async Task<AssetReadDto> GetAsync(long assetId, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Assets.get<AssetReadDto>(assetId);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="query">The query of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(AssetDeleteDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Assets.delete<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple assets in the same project by identity items.
        /// </summary>
        /// <param name="items">The list of assets identities to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> items, CancellationToken token = default)
        {
            var query = new AssetDeleteDto() { Items = items };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple assets in the same project by internal ids.
        /// </summary>
        /// <param name="internalIds">The list of assets ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var query = new AssetDeleteDto() { Items = internalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple assets in the same project by external ids.
        /// </summary>
        /// <param name="externalIds">The list of assets ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var query = new AssetDeleteDto() { Items = externalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }
        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed per
        /// request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of assets identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<AssetReadDto>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Assets.retrieve<IEnumerable<AssetReadDto>>(ids);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed per
        /// request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<AssetReadDto>> RetrieveAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple assets in the same project. A maximum of 1000 assets IDs may be listed per
        /// request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of assets internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<AssetReadDto>> RetrieveAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
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
        public async Task<IEnumerable<AssetReadDto>> SearchAsync (AssetSearchDto query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.Assets.search<IEnumerable<AssetReadDto>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<IEnumerable<AssetReadDto>> UpdateAsync (IEnumerable<AssetUpdateItem> query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.Assets.update<IEnumerable<AssetReadDto>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}