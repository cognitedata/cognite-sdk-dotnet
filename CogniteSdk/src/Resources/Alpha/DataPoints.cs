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

namespace CogniteSdk.Resources.Alpha
{
    /// <summary>
    /// For internal use. Contains all data points methods with support for instance identifiers.
    /// </summary>
    public class DataPointsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal DataPointsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of time series data points matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series data points matching given filters and optional cursor</returns>
        public async Task<DataPointListResponse> ListAsync(CogniteSdk.Alpha.DataPointsQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Alpha.DataPoints.list(query, GetContext(token));
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

            var req = Oryx.Cognite.Alpha.DataPoints.create(points, GetContext(token));
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

            var req = Oryx.Cognite.Alpha.DataPoints.createWithGzip(points, compression, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete time series data points
        /// </summary>
        /// <param name="query">Query to specify range of data points to delete from time series.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> DeleteAsync(CogniteSdk.Alpha.DataPointsDelete query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Alpha.DataPoints.delete(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the latest data point in the given time series
        /// </summary>
        /// <param name="query">The latest query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of latest data points.</returns>
        public async Task<IEnumerable<CogniteSdk.Alpha.DataPointsItem<DataPoint>>> LatestAsync(CogniteSdk.Alpha.DataPointsLatestQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Alpha.DataPoints.latest(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}