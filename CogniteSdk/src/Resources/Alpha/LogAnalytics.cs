// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Alpha;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources.Alpha
{
    /// <summary>
    /// Contains methods for industrial log analytics.
    /// </summary>
    public class LogAnalyticsResource : Resource
    {
        internal LogAnalyticsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Creates a list of logs in the provided stream.
        /// </summary>
        /// <param name="stream">Stream to ingest logs into.</param>
        /// <param name="logs">Logs to ingest.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task IngestAsync(string stream, IEnumerable<LogItem> logs, CancellationToken token = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var req = Oryx.Cognite.Alpha.LogAnalytics.ingest(stream, new LogIngest
            {
                Items = logs,
            }, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of logs.
        /// </summary>
        /// <typeparam name="T">Type of properties in the retrieved logs.</typeparam>
        /// <param name="stream">Stream to ingest logs into.</param>
        /// <param name="request">Log retrieval request.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Retrieved logs.</returns>
        public async Task<IEnumerable<Log<T>>> RetrieveAsync<T>(string stream, LogRetrieve request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Alpha.LogAnalytics.retrieve<T>(stream, request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronizes updates to logs. This endpoint will always return a cursor.
        /// </summary>
        /// <typeparam name="T">Type of properties in the retrieved logs.</typeparam>
        /// <param name="stream">Stream to ingest logs into.</param>
        /// <param name="request">Log retreival request.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sync response.</returns>
        public async Task<LogSyncResponse<T>> SyncAsync<T>(string stream, LogSync request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Alpha.LogAnalytics.sync<T>(stream, request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a new stream. A stream is a target for high volume data ingestion,
        /// with data shaped by the Data Modeling concepts.
        /// For beta we allow each project to create up to 10 streams.
        /// </summary>
        /// <param name="stream">Stream to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created stream</returns>
        public async Task<Stream> CreateStreamAsync(StreamWrite stream, CancellationToken token = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var req = Oryx.Cognite.Alpha.LogAnalytics.createStream(stream, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a stream by its identifier.
        /// </summary>
        /// <param name="stream">Stream to delete</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task DeleteStreamAsync(string stream, CancellationToken token = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var req = Oryx.Cognite.Alpha.LogAnalytics.deleteStream(stream, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List all streams in the project.
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Listed streams.</returns>
        public async Task<IEnumerable<Stream>> ListStreamsAsync(CancellationToken token = default)
        {
            var req = Oryx.Cognite.Alpha.LogAnalytics.listStreams(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a stream by its identifier.
        /// </summary>
        /// <param name="stream">Stream to retrieve</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved stream</returns>
        public async Task<Stream> RetrieveStreamAsync(string stream, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Alpha.LogAnalytics.retrieveStream(stream, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}