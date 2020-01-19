// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.DataPoints;
using static Oryx.Cognite.HandlerModule;
using CogniteSdk.TimeSeries;
using Com.Cognite.V1.Timeseries.Proto;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all data points methods.
    /// </summary>
    public class DataPoints
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal DataPoints(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<DataPointListResponse> ListAsync(DataPointsQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.DataPoints.list<DataPointListResponse>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Create assets.
        /// </summary>
        /// <param name="assets">Time series to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TimeSeriesReadDto>> CreateAsync(IEnumerable<TimeSeriesWriteDto> assets, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.create<IEnumerable<TimeSeriesReadDto>>(assets);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="query">The list of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(TimeSeriesDeleteDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.delete<EmptyResponse>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Retrieves information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<TimeSeriesReadDto>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.retrieve<IEnumerable<TimeSeriesReadDto>>(ids);
            return await runUnsafeAsync(req, _ctx, token);
        }
        
        /// <summary>
        /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<TimeSeriesReadDto>> SearchAsync (SearchQueryDto<TimeSeriesFilterDto, SearchDto> query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.TimeSeries.search<IEnumerable<TimeSeriesReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Updates multiple timeseries within the same project. This operation supports partial updates, meaning that
        /// fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of assets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<IEnumerable<TimeSeriesReadDto>> UpdateAsync (IEnumerable<UpdateItemType<TimeSeriesUpdateDto>> query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.TimeSeries.update<IEnumerable<TimeSeriesReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }
    }
}