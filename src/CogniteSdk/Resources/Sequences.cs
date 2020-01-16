// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Sequences;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all sequences methods.
    /// </summary>
    public class Sequences
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal Sequences(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of sequences matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<SequenceReadDto>> ListAsync(SequenceQueryDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.list<ItemsWithCursor<SequenceReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Create sequences.
        /// </summary>
        /// <param name="sequences">Sequences to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<ItemsWithoutCursor<SequenceReadDto>> CreateAsync(ItemsWithoutCursor<SequenceWriteDto> sequences, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.create<ItemsWithoutCursor<SequenceReadDto>>(sequences);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Delete multiple sequences in the same project.
        /// </summary>
        /// <param name="query">The list of sequeneces to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(ItemsWithoutCursor<Identity> query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.delete<EmptyResponse>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed per
        /// request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of events to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<ItemsWithoutCursor<SequenceReadDto>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Sequences.retrieve<ItemsWithoutCursor<SequenceReadDto>>(ids);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Retrieves a list of sequences matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<ItemsWithoutCursor<SequenceReadDto>> SearchAsync (SearchQueryDto<SequenceFilterDto, SearchDto> query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.Sequences.search<ItemsWithoutCursor<SequenceReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of events to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<ItemsWithoutCursor<SequenceReadDto>> UpdateAsync (ItemsWithoutCursor<UpdateItem<SequenceUpdateDto>> query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.Sequences.update<ItemsWithoutCursor<SequenceReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }
    }
}