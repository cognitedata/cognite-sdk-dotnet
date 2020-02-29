// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Sequences;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all sequences methods.
    /// </summary>
    public class SequencesResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal SequencesResource(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of sequences matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<Sequence>> ListAsync(SequenceQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.list<ItemsWithCursor<Sequence>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create sequences.
        /// </summary>
        /// <param name="sequences">Sequences to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<SequenceDataRead>> CreateAsync(IEnumerable<SequenceCreate> sequences, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.create<IEnumerable<SequenceDataRead>>(sequences);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple sequences in the same project.
        /// </summary>
        /// <param name="query">The list of sequence identities to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.delete<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple sequences in the same project.
        /// </summary>
        /// <param name="internalIds">The list of sequence internal ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var req = internalIds.Select(Identity.Create);
            return await DeleteAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple sequences in the same project.
        /// </summary>
        /// <param name="internalIds">The list of sequence external ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> internalIds, CancellationToken token = default)
        {
            var req = internalIds.Select(Identity.Create);
            return await DeleteAsync(req, token).ConfigureAwait(false);
        }
        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of events identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Sequence>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.retrieve<IEnumerable<Sequence>>(ids);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of events internal ids to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Sequence>> RetrieveAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var req = internalIds.Select(Identity.Create);
            return await RetrieveAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed per
        /// request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of events external ids to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Sequence>> RetrieveAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var req = externalIds.Select(Identity.Create);
            return await RetrieveAsync(req, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Retrieves a list of sequences matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<Sequence>> SearchAsync (SequenceSearch query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.search<IEnumerable<Sequence>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of events to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<IEnumerable<Sequence>> UpdateAsync (IEnumerable<UpdateItem<SequenceUpdate>> query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.update<IEnumerable<Sequence>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes data requests, and returns the result. NB - This operation uses a dynamic limit on the number of
        /// rows returned based on the number and type of columns, use the provided cursor to paginate and retrieve all
        /// data.
        /// </summary>
        /// <param name="query">The sequence rows query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence read data.</returns>
        public async Task<SequenceDataRead> ListRowsAsync(SequenceRowQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.listRows<SequenceDataRead>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create rows.
        /// </summary>
        /// <param name="query">Query of rows to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> CreateRowsAsync(IEnumerable<SequenceDataWrite> query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.createRows<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete rows
        /// </summary>
        /// <param name="query">Sequence of rows to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> DeleteRowsAsync(IEnumerable<SequenceRowDelete> query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.deleteRows<EmptyResponse>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}