// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Oryx;
using Oryx.Cognite;

using CogniteSdk.Types.Assets;
using CogniteSdk.Types.Common;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;
using AssetItemsReadDto = CogniteSdk.Types.Common.ResourceItems<CogniteSdk.Types.Assets.AssetReadDto>;
using AssetItemsWriteDto = CogniteSdk.Types.Common.ResourceItems<CogniteSdk.Types.Assets.AssetWriteDto>;

using AssetItemsWithCursorReadDto = CogniteSdk.Types.Common.ResourceItemsWithCursor<CogniteSdk.Types.Assets.AssetReadDto>;


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
        /// Retrieves list of assets matching query, filter, and a cursor if given limit is exceeded
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
            var ctx = Context.setCancellationToken(token, this._ctx);
            var req = Oryx.Cognite.Assets.create<AssetItemsReadDto>(assets);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<AssetItemsReadDto>(result.ErrorValue);
        }

        /*
    
        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <returns>Asset with the given id.</returns>
        [<Extension>]
        static member GetAsync (this: ClientExtension, assetId: int64, [<Optional>] token: CancellationToken) : Task<AssetReadDto> =
            task {
                let ctx = this.Ctx |> Context.setCancellationToken token
                let! result = Entity.getAsync assetId ctx
                match result with
                | Ok ctx -> return ctx.Response
                | Error error -> return raiseError error
            }
    
    }
    
    [<Extension>]
    type DeleteAssetsExtensions =
        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="assets">The list of assets to delete.</param>
        /// <param name="recursive">If true, delete all children recursively.</param>
        [<Extension>]
        static member DeleteAsync(this: ClientExtension, ids: Identity seq, recursive: bool, [<Optional>] token: CancellationToken) : Task =
            task {
                let ctx = this.Ctx |> Context.setCancellationToken token
                let! result = Delete.deleteAsync (ids, recursive) ctx
                match result with
                | Ok _ -> return ()
                | Error error -> return raiseError error
            } :> _
    
        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="assets">The list of assets to delete.</param>
        /// <param name="recursive">If true, delete all children recursively.</param>
        [<Extension>]
        static member DeleteAsync(this: ClientExtension, ids: int64 seq, recursive: bool, [<Optional>] token: CancellationToken) : Task =
            this.DeleteAsync(ids |> Seq.map Identity.Id, recursive, token)
    
        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="assets">The list of assets to delete.</param>
        /// <param name="recursive">If true, delete all children recursively.</param>
        [<Extension>]
        static member DeleteAsync(this: ClientExtension, ids: string seq, recursive: bool, [<Optional>] token: CancellationToken) : Task =
            this.DeleteAsync(ids |> Seq.map Identity.ExternalId, recursive, token)
    
        */
    }
}