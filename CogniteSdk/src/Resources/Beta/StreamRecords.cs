// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Beta;
using CogniteSdk.DataModels;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources.Beta
{
    /// <summary>
    /// Contains methods for stream records.
    /// </summary>
    public class StreamRecordsResource : Resource
    {
        internal StreamRecordsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Creates a list of records in the provided stream.
        /// </summary>
        /// <param name="stream">Stream to ingest records into.</param>
        /// <param name="records">Records to ingest.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task IngestAsync(string stream, IEnumerable<StreamRecordWrite> records, CancellationToken token = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var req = Oryx.Cognite.Beta.StreamRecords.ingest(stream, new StreamRecordIngest
            {
                Items = records,
            }, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Upsert (create or update) records in the provided mutable stream.
        /// </summary>
        /// <param name="stream">Stream to upsert records into.</param>
        /// <param name="records">Records to upsert.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task UpsertAsync(string stream, IEnumerable<StreamRecordWrite> records, CancellationToken token = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var req = Oryx.Cognite.Beta.StreamRecords.upsert(stream, new StreamRecordIngest
            {
                Items = records,
            }, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete records from the provided mutable stream.
        /// </summary>
        /// <param name="stream">Stream to delete records from.</param>
        /// <param name="recordIds">Record identifiers to delete.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task DeleteRecordsAsync(string stream, IEnumerable<InstanceIdentifier> recordIds, CancellationToken token = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var req = Oryx.Cognite.Beta.StreamRecords.delete(stream, new StreamRecordDelete
            {
                Items = recordIds,
            }, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of records.
        /// </summary>
        /// <typeparam name="T">Type of properties in the retrieved records.</typeparam>
        /// <param name="stream">Stream to ingest records into.</param>
        /// <param name="request">record retrieval request.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Retrieved records.</returns>
        public async Task<IEnumerable<StreamRecord<T>>> RetrieveAsync<T>(string stream, StreamRecordsRetrieve request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Beta.StreamRecords.retrieve<T>(stream, request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronizes updates to records. This endpoint will always return a cursor.
        /// </summary>
        /// <typeparam name="T">Type of properties in the retrieved records.</typeparam>
        /// <param name="stream">Stream to ingest records into.</param>
        /// <param name="request">record retreival request.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sync response.</returns>
        public async Task<StreamRecordsSyncResponse<T>> SyncAsync<T>(string stream, StreamRecordsSync request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Beta.StreamRecords.sync<T>(stream, request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Aggregate records from a stream.
        /// </summary>
        /// <param name="stream">Stream to aggregate records from.</param>
        /// <param name="request">Aggregation request.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Aggregation response.</returns>
        public async Task<StreamRecordsAggregateResponse> AggregateAsync(string stream, StreamRecordsAggregate request, CancellationToken token = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Beta.StreamRecords.aggregate(stream, request, GetContext(token));
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

            var req = Oryx.Cognite.Beta.StreamRecords.createStream(stream, GetContext(token));
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

            var req = Oryx.Cognite.Beta.StreamRecords.deleteStream(stream, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List all streams in the project.
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Listed streams.</returns>
        public async Task<IEnumerable<Stream>> ListStreamsAsync(CancellationToken token = default)
        {
            var req = Oryx.Cognite.Beta.StreamRecords.listStreams(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a stream by its identifier.
        /// </summary>
        /// <param name="stream">Stream to retrieve</param>
        /// <param name="includeStatistics">If true, usage statistics will be returned together with stream settings.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved stream</returns>
        public async Task<Stream> RetrieveStreamAsync(string stream, bool? includeStatistics = null, CancellationToken token = default)
        {
            var includeStatsOption = includeStatistics.HasValue
                ? FSharpOption<bool>.Some(includeStatistics.Value)
                : FSharpOption<bool>.None;

            var req = Oryx.Cognite.Beta.StreamRecords.retrieveStream(stream, includeStatsOption, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
