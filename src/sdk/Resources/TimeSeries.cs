// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Oryx;
using Oryx.Cognite;

using CogniteSdk.TimeSeries;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;
using System.Collections.Generic;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all asset methods.
    /// </summary>
    public class TimeSeries
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal TimeSeries(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<TimeSeriesReadDto>> ListAsync(TimeSeriesQuery query, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, this._ctx);
            var req = Oryx.Cognite.TimeSeries.list<ItemsWithCursor<TimeSeriesReadDto>>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithCursor<TimeSeriesReadDto>>(result.ErrorValue);
        }

        /// <summary>
        /// Create assets.
        /// </summary>
        /// <param name="assets">Time series to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<ItemsWithoutCursor<TimeSeriesReadDto>> CreateAsync(ItemsWithoutCursor<TimeSeriesWriteDto> assets, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.TimeSeries.create<ItemsWithoutCursor<TimeSeriesReadDto>>(assets);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithoutCursor<TimeSeriesReadDto>>(result.ErrorValue);
        }

        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="query">The list of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async void DeleteAsync(TimeSeriesDeleteDto query, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.TimeSeries.delete<HttpResponseMessage>(query);

            var result = await Handler.runAsync(req, ctx);
            if (result.IsError)
            {
                HandlersModule.raiseError<HttpResponseMessage>(result.ErrorValue);
            }
        }

        /// <summary>
        /// Retrieves information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<ItemsWithoutCursor<TimeSeriesReadDto>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.TimeSeries.retrieve<ItemsWithoutCursor<TimeSeriesReadDto>>(ids);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithoutCursor<TimeSeriesReadDto>>(result.ErrorValue);
        }
        
        /// <summary>
        /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<ItemsWithoutCursor<TimeSeriesReadDto>> SearchAsync (TimeSeriesSearchQueryDto query, CancellationToken token = default )
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.TimeSeries.search<ItemsWithoutCursor<TimeSeriesReadDto>>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithoutCursor<TimeSeriesReadDto>>(result.ErrorValue);
        }

        /// <summary>
        /// Updates multiple timeseries within the same project. This operation supports partial updates, meaning that
        /// fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<ItemsWithoutCursor<TimeSeriesReadDto>> UpdateAsync (ItemsWithoutCursor<UpdateItem<TimeSeriesUpdateDto>> query, CancellationToken token = default )
        {
            var ctx = Context.setCancellationToken(token, _ctx);
            var req = Oryx.Cognite.TimeSeries.update<ItemsWithoutCursor<TimeSeriesReadDto>>(query);

            var result = await Handler.runAsync(req, ctx);
            return result.IsOk ? result.ResultValue : HandlersModule.raiseError<ItemsWithoutCursor<TimeSeriesReadDto>>(result.ErrorValue);
        }
    }
}