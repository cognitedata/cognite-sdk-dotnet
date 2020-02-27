// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Com.Cognite.V1.Timeseries.Proto;
using static Oryx.Cognite.HandlerModule;
using CogniteSdk;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all data points methods.
    /// </summary>
    public class DataPointsResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal DataPointsResource(HttpContext ctx)
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
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create data points.
        /// </summary>
        /// <param name="points">Data Points to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> CreateAsync(DataPointInsertionRequest points, CancellationToken token = default)
        {
            var req = Oryx.Cognite.DataPoints.create<EmptyResponse>(points);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="query">The list of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> DeleteAsync(DataPointsDelete query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.DataPoints.delete<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the latest datapoint in the given time series
        /// </summary>
        /// <param name="query">The latest query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of latest data points.</returns>
        public async Task<IEnumerable<DataPointsItem<DataPoint>>> LatestAsync(DataPointsLatestQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.DataPoints.latest<IEnumerable<DataPointsItem<DataPoint>>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}