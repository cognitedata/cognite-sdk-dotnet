// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

using Com.Cognite.V1.Timeseries.Proto;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Pipeline;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all data points methods.
    /// </summary>
    public class DataPointsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal DataPointsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<DataPointListResponse> ListAsync(DataPointsQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.DataPoints.list(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create data points.
        /// </summary>
        /// <param name="points">Data Points to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> CreateAsync(DataPointInsertionRequest points, CancellationToken token = default)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            var req = Oryx.Cognite.DataPoints.create(points, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create data points, applying Gzip compression at level <paramref name="compression"/>.
        /// </summary>
        /// <param name="points">Data Points to create</param>
        /// <param name="compression">Compression level</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response</returns>
        public async Task<EmptyResponse> CreateAsync(
            DataPointInsertionRequest points,
            CompressionLevel compression,
            CancellationToken token = default)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            var req = Oryx.Cognite.DataPoints.createWithGzip(points, compression, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
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
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.DataPoints.delete(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the latest datapoint in the given time series
        /// </summary>
        /// <param name="query">The latest query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of latest data points.</returns>
        public async Task<IEnumerable<DataPointsItem<DataPoint>>> LatestAsync(DataPointsLatestQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.DataPoints.latest(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}