// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all event methods.
    /// </summary>
    public class EventsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal EventsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Asynchronously retrieve a list of event like objects matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <typeparam name="T">Type of event to return, e.g Event or EventWithoutMetadata.</typeparam>
        /// <returns>List of events matching given filters and optional cursor</returns>
        public async Task<IItemsWithCursor<T>> ListAsync<T>(EventQuery query, CancellationToken token = default) where T : Event
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.list<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of events matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of events matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<Event>> ListAsync(EventQuery query, CancellationToken token = default)
        {
            return (ItemsWithCursor<Event>)await ListAsync<Event>(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves number of events matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of events matching given filters</returns>
        public async Task<int> AggregateAsync(EventQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.aggregate(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously create events.
        /// </summary>
        /// <param name="events">Events to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created events.</returns>
        public async Task<IEnumerable<Event>> CreateAsync(IEnumerable<EventCreate> events, CancellationToken token = default)
        {
            if (events is null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            var req = Oryx.Cognite.Events.create<Event>(events, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about an event given an event id.
        /// </summary>
        /// <param name="eventId">The id of the event to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <typeparam name="T">Type of event to return, e.g Event or EventWithoutMetadata.</typeparam>
        /// <returns>Event with the given id.</returns>
        public async Task<T> GetAsync<T>(long eventId, CancellationToken token = default) where T : Event
        {
            var req = Oryx.Cognite.Events.get<T>(eventId, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about an event given an event id.
        /// </summary>
        /// <param name="eventId">The id of the event to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Event with the given id.</returns>
        public async Task<Event> GetAsync(long eventId, CancellationToken token = default)
        {
            return await GetAsync<Event>(eventId, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Asynchronously delete multiple events in the same project.
        /// </summary>
        /// <param name="query">The list of events to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(EventDelete query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.delete(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously delete multiple events in the same project.
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
        /// Asynchronously delete multiple events in the same project.
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
        /// Asynchronously delete multiple events in the same project.
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
        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Asynchronously retrieves information about multiple events in the same project. A maximum of 1000 events IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of events to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <typeparam name="T">Type of event to return, e.g Event or EventWithoutMetadata.</typeparam>
        /// <returns>A sequence of the requested events.</returns>
        public async Task<IEnumerable<T>> RetrieveAsync<T>(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : Event
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Oryx.Cognite.Events.retrieve<T>(ids, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple events in the same project. A maximum of 1000 events IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of events to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested events.</returns>
        public async Task<IEnumerable<Event>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return await RetrieveAsync<Event>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple events in the same project. A maximum of 1000 events IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of event internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested events.</returns>
        public async Task<IEnumerable<T>> RetrieveAsync<T>(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : Event
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var req = internalIds.Select(Identity.Create);
            return await RetrieveAsync<T>(req, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple events in the same project. A maximum of 1000 events IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of event internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested events.</returns>
        public async Task<IEnumerable<Event>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return await RetrieveAsync<Event>(internalIds, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple events in the same project. A maximum of 1000 events IDs
        /// may be listed per request and all of them must be unique.
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
            return await RetrieveAsync<Event>(req, ignoreUnknownIds, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Asynchronously retrieves a list of event like objects matching the given criteria. This operation does not
        /// support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of events matching given criteria.</returns>
        public async Task<IEnumerable<T>> SearchAsync<T>(EventSearch query, CancellationToken token = default) where T : Event
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.search<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves a list of events matching the given criteria. This operation does not support
        /// pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of events matching given criteria.</returns>
        public async Task<IEnumerable<Event>> SearchAsync(EventSearch query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return await SearchAsync<Event>(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously update one or more events. Supports partial updates, meaning that fields omitted from the
        /// requests are not changed
        /// </summary>
        /// <param name="query">The list of events to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated events.</returns>
        public async Task<IEnumerable<Event>> UpdateAsync(IEnumerable<EventUpdateItem> query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Events.update<Event>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
