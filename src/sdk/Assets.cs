// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Net.Http;
using System.Linq;
using Oryx;

namespace CogniteSdk
{
    public class Assets
    {
        async Task<IEnumerable<AssetEntity>> CreateAsync(IEnumerable<AssetEntity> assets, CancellationToken? token)
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
