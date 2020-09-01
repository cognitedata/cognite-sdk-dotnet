// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all event methods.
    /// </summary>
    public class EventsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="includeMetadata">Include meta-data in responses or not.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal EventsResource(Func<CancellationToken, Task<string>> authHandler, bool includeMetadata, HttpContext ctx) : base(authHandler, includeMetadata, ctx)
        {
        }

        /// <summary>
        /// Asynchronously retrieves list of events matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<Event>> ListAsync(EventQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.list<Event, ItemsWithCursor<Event>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of events matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public ItemsWithCursor<Event> List(EventQuery query, CancellationToken token = default)
        {
            return ListAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously retrieves number of events matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of assets matching given filters</returns>
        public async Task<int> AggregateAsync(EventQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.aggregate<Int32>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves number of events matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of assets matching given filters</returns>
        public int Aggregate(EventQuery query, CancellationToken token = default)
        {
            return AggregateAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously create events.
        /// </summary>
        /// <param name="events">Events to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Event>> CreateAsync(IEnumerable<EventCreate> events, CancellationToken token = default)
        {
            if (events is null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            var req = Oryx.Cognite.Events.create<Event, IEnumerable<Event>>(events);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create events.
        /// </summary>
        /// <param name="events">Events to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public IEnumerable<Event> Create(IEnumerable<EventCreate> events, CancellationToken token = default)
        {
            return CreateAsync(events, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="eventId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public async Task<Event> GetAsync(long eventId, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Events.get<Event, Event>(eventId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about an asset given an asset id.
        /// </summary>
        /// <param name="eventId">The id of the asset to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Asset with the given id.</returns>
        public Event Get(long eventId, CancellationToken token = default)
        {
            return GetAsync(eventId, token).GetAwaiter().GetResult();
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="query">The list of events to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(EventDelete query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="query">The list of events to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(EventDelete query, CancellationToken token = default)
        {
            return DeleteAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="identities">The list of event ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> identities, CancellationToken token = default)
        {
            if (identities is null)
            {
                throw new ArgumentNullException(nameof(identities));
            }

            var query = new EventDelete { Items = identities };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="identities">The list of event ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<Identity> identities, CancellationToken token = default)
        {
            return DeleteAsync(identities, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="ids">The list of event ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var query = new EventDelete { Items = ids.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="ids">The list of event ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<long> ids, CancellationToken token = default)
        {
            return DeleteAsync(ids, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="externalIds">The list of event externalids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var query = new EventDelete { Items = externalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="externalIds">The list of event externalids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            return DeleteAsync(externalIds, token).GetAwaiter().GetResult();
        }

        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of events to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Event>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Oryx.Cognite.Events.retrieve<Event, IEnumerable<Event>>(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of events to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<Event> Retrieve(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(ids, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of event internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Event>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var req = internalIds.Select(Identity.Create);
            return await RetrieveAsync(req, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of event internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<Event> Retrieve(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(internalIds, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of event external ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<Event>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var req = externalIds.Select(Identity.Create);
            return await RetrieveAsync(req, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple events in the same project. A maximum of 1000 events IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of event external ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<Event> Retrieve(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(externalIds, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        #endregion

        /// <summary>
        /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<Event>> SearchAsync (EventSearch query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.search<Event, IEnumerable<Event>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public IEnumerable<Event> Search(EventSearch query, CancellationToken token = default)
        {
            return SearchAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="query">The list of events to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public async Task<IEnumerable<Event>> UpdateAsync (IEnumerable<EventUpdateItem> query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.update<Event, IEnumerable<Event>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="query">The list of events to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated assets.</returns>
        public IEnumerable<Event> Update(IEnumerable<EventUpdateItem> query, CancellationToken token = default)
        {
            return UpdateAsync(query, token).GetAwaiter().GetResult();
        }
    }
}