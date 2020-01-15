// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Events;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all event methods.
    /// </summary>
    public class Events
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal Events(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of events matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<EventReadDto>> ListAsync(EventQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Events.list<ItemsWithCursor<EventReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Create events.
        /// </summary>
        /// <param name="events">Events to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<ItemsWithoutCursor<EventReadDto>> CreateAsync(ItemsWithoutCursor<EventWriteDto> events, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Events.create<ItemsWithoutCursor<EventReadDto>>(events);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="eventId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public async Task<EventReadDto> GetAsync(long eventId, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Events.get<EventReadDto>(eventId);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Delete multiple events in the same project, along with all their descendants in the asset hierarchy if recursive is true.
        /// </summary>
        /// <param name="query">The list of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(EventDeleteDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Events.delete<EmptyResponse>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed per
        /// request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of events to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<ItemsWithoutCursor<EventReadDto>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Events.retrieve<ItemsWithoutCursor<EventReadDto>>(ids);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<ItemsWithoutCursor<EventReadDto>> SearchAsync (SearchQueryDto<EventFilterDto, SearchDto> query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.Events.search<ItemsWithoutCursor<EventReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }

        /// <summary>
        /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of events to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<ItemsWithoutCursor<EventReadDto>> UpdateAsync (ItemsWithoutCursor<UpdateItem<EventUpdateDto>> query, CancellationToken token = default )
        {
            var req = Oryx.Cognite.Events.update<ItemsWithoutCursor<EventReadDto>>(query);
            return await runUnsafeAsync(req, _ctx, token);
        }
    }
}