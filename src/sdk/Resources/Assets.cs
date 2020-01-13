// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Oryx;
using Oryx.Cognite;

using CogniteSdk.Assets;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;


namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all asset methods.
    /// </summary>
    public class Assets
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal Assets(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of assets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<AssetReadDto>> ListAsync(AssetQuery query, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, this._ctx);
            var req = Oryx.Cognite.Assets.list<ItemsWithCursor<AssetReadDto>>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithCursor<AssetReadDto>>(result.ErrorValue);
        }

        /// <summary>
        /// Create assets.
        /// </summary>
        /// <param name="assets">Assets to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<ItemsWithoutCursor<AssetReadDto>> CreateAsync(ItemsWithoutCursor<AssetWriteDto> assets, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.create<ItemsWithoutCursor<AssetReadDto>>(assets);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithoutCursor<AssetReadDto>>(result.ErrorValue);
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public async Task<AssetReadDto> GetAsync(long assetId, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.get<AssetReadDto>(assetId);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<AssetReadDto>(result.ErrorValue);
        }

        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="query">The list of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async void DeleteAsync(AssetDeleteDto query, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.delete<HttpResponseMessage>(query);

            var result = await Handler.runAsync(req, ctx);
            if (result.IsError)
            {
                HandlersModule.raiseError<HttpResponseMessage>(result.ErrorValue);
            }
        }

        /// <summary>
        /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<ItemsWithoutCursor<AssetReadDto>> SearchAsync (AssetSearchQueryDto query, CancellationToken token = default )
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.search<ItemsWithoutCursor<AssetReadDto>>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithoutCursor<AssetReadDto>>(result.ErrorValue);
        }

        /// <summary>
        /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<ItemsWithoutCursor<AssetReadDto>> UpdateAsync (ItemsWithoutCursor<UpdateItem<AssetUpdateDto>> query, CancellationToken token = default )
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.update<ItemsWithoutCursor<AssetReadDto>>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithoutCursor<AssetReadDto>>(result.ErrorValue);
        }
    }
}