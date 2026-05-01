// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Com.Cognite.V1.Timeseries.Proto;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources.Beta
{
    /// <summary>
    /// Beta time series data point operations
    /// </summary>
    public class DataPointsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal DataPointsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieve data points using the beta API
        /// </summary>
        /// <param name="query">Data points query.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<DataPointListResponse> ListAsync(DataPointsQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Beta.DataPoints.list(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the latest data points using the beta API
        /// </summary>
        /// <param name="query">The latest query.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<DataPointListResponse> LatestAsync(DataPointsLatestQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Beta.DataPoints.latest(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert data points using the beta API
        /// </summary>
        /// <param name="points">Data points to insert.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> CreateAsync(DataPointInsertionRequest points, CancellationToken token = default)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            var req = Oryx.Cognite.Beta.DataPoints.create(points, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert data points using the beta API, gzip-compressed at the given level
        /// </summary>
        /// <param name="points">Data points to insert.</param>
        /// <param name="compression">Gzip compression level.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> CreateAsync(
            DataPointInsertionRequest points,
            CompressionLevel compression,
            CancellationToken token = default)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            var req = Oryx.Cognite.Beta.DataPoints.createWithGzip(points, compression, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
