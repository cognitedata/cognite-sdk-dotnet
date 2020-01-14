// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Raw;
using static Oryx.Cognite.HandlersModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all raw methods.
    /// </summary>
    public class Raw
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal Raw(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of RAW databases matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of databases matching given filters, and optional cursor</returns>
        public async Task<ItemsWithCursor<DatabaseDto>> ListDatabasesAsync(DatabaseQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Raw.listDatabases<ItemsWithCursor<DatabaseDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }
    }
}