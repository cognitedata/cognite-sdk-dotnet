// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Oryx;
using Oryx.Cognite;

using CogniteSdk.Types.Assets;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;
using AssetItemsReadDto = CogniteSdk.Types.Common.ResourceItems<CogniteSdk.Types.Assets.AssetReadDto>;
using AssetItemsWriteDto = CogniteSdk.Types.Common.ResourceItems<CogniteSdk.Types.Assets.AssetWriteDto>;
using AssetItemsWithCursorReadDto = CogniteSdk.Types.Common.ResourceItemsWithCursor<CogniteSdk.Types.Assets.AssetReadDto>;
using AssetItemsUpdateDto = CogniteSdk.Types.Common.ResourceItems<CogniteSdk.Types.Common.UpdateItem<CogniteSdk.Types.Assets.AssetUpdateDto>>;


namespace CogniteSdk
{
    /// <summary>
    /// Contains all asset methods.
    /// </summary>
    public class Assets
    {
        private readonly HttpContext _ctx;

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
        public async Task<AssetItemsWithCursorReadDto> ListAsync(AssetQuery query, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, this._ctx);
            var req = Oryx.Cognite.Assets.list<AssetItemsWithCursorReadDto>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<AssetItemsWithCursorReadDto>(result.ErrorValue);
        }

        /// <summary>
        /// Create assets.
        /// </summary>
        /// <param name="assets">Assets to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<AssetItemsReadDto> CreateAsync(AssetItemsWriteDto assets, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.create<AssetItemsReadDto>(assets);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<AssetItemsReadDto>(result.ErrorValue);
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
        public async Task<AssetItemsReadDto> SearchAsync (AssetSearchQueryDto query, CancellationToken token = default )
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.search<AssetItemsReadDto>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<AssetItemsReadDto>(result.ErrorValue);
        }
        
        /// <summary>
        /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<AssetItemsReadDto> UpdateAsync (AssetItemsUpdateDto query, CancellationToken token = default )
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.Assets.update<AssetItemsReadDto>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<AssetItemsReadDto>(result.ErrorValue);
        }
    }
}