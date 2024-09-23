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

            var req = Oryx.Cognite.Alpha.LogAnalytics.ingest(new LogIngest
            {
                Stream = stream,
                Items = logs,
            }, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of logs.
        /// </summary>
        /// <typeparam name="T">Type of properties in the retrieved logs.</typeparam>
        /// <param name="request">Log retrieval request.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Retrieved logs.</returns>
        public async Task<IEnumerable<Log<T>>> RetrieveAsync<T>(LogRetrieve request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Alpha.LogAnalytics.retrieve<T>(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronizes updates to logs. This endpoint will always return a cursor.
        /// </summary>
        /// <typeparam name="T">Type of properties in the retrieved logs.</typeparam>
        /// <param name="request">Log retreival request.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sync response.</returns>
        public async Task<LogSyncResponse<T>> SyncAsync<T>(LogSync request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Alpha.LogAnalytics.sync<T>(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}