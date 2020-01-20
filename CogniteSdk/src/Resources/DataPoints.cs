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
            return await runUnsafeAsync(_ctx, token, req);
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
            return await runUnsafeAsync(_ctx, token, req);
        }

        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="query">The list of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(TimeSeriesDeleteDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.delete<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req);
        }
    }
}