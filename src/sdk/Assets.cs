// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Oryx;
using Oryx.Cognite;

using CogniteSdk.Types;
using CogniteSdk.Types.Assets;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;
using AssetItemsReadDto = CogniteSdk.Types.Common.ResourceItemsWithCursor<CogniteSdk.Types.Assets.AssetReadDto>;

namespace CogniteSdk
{
    public class Assets
    {
        private readonly HttpContext ctx;

        public Assets(HttpContext ctx)
        {
            this.ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of assets matching query, filter, and a cursor if given limit is exceeded
        /// </summary>
        /// <param name="options">Optional limit and cursor</param>
        /// <param name="filters">Search filters</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<AssetItemsReadDto> ListAsync(IEnumerable<AssetQuery> options, IEnumerable<AssetFilter> filters, CancellationToken? token)
        {
                var ctx = Context.setCancellationToken(token, this.ctx);
                let req = Oryx.Cognite.Assets.list(options, filters);
                let! result = await runAsync(req, ctx);
                match result with
                | Ok items -> return items
                | Error error -> return raiseError error
        }

        public async Task<IEnumerable<Asset>> CreateAsync(IEnumerable<Asset> assets, CancellationToken? token)
        {
                var assets = assets.Select(AssetWriteDto.FromAssetEntity);
                var ctx = Context.setCancellationToken(token, this.Ctx);
                var result = await Create.createAsync(assets, ctx);

                //match result with
                //| Ok ctx -> return ctx.Response |> Seq.map (fun asset -> asset.ToAssetEntity ())
                //| Error error -> return raiseError error
                return null;
        }
    }
}
