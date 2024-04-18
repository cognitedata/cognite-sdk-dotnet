// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using CogniteSdk.Beta;

using Oryx;
using Oryx.Cognite.Beta;

using Microsoft.FSharp.Core;

namespace CogniteSdk.Resources.Beta
{
    /// <summary>
    /// Resource for timeseries subscriptions
    /// </summary>
    public class SubscriptionsResource : Resource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SubscriptionsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create a list of datapoint subscriptions.
        /// </summary>
        /// <param name="subscriptions">Subscriptions to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created subscriptions</returns>
        public async Task<IEnumerable<Subscription>> CreateAsync(IEnumerable<SubscriptionCreate> subscriptions, CancellationToken token = default)
        {
            var req = Subscriptions.create(subscriptions, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of datapoint subscriptions, optionally ignoring unknown IDs
        /// </summary>
        /// <param name="items">External IDs of subscriptions to delete</param>
        /// <param name="ignoreUnknownIds">True to ignore IDs not present in CDF,
        /// if this is false and subscriptions are missing, the request will fail.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(IEnumerable<string> items, bool ignoreUnknownIds = false, CancellationToken token = default)
        {
            var req = Subscriptions.delete(items, ignoreUnknownIds, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the next batch of data from one or more subscription partitions.
        /// </summary>
        /// <param name="query">Data retrieval query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Any changes to the subscription partition(s) in the request</returns>
        public async Task<SubscriptionDataResponse> ListDataAsync(ListSubscriptionData query, CancellationToken token = default)
        {
            var req = Subscriptions.listData(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List timeseries in the given subscription, with pagination.
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>A list of timeseries IDs and a cursor for pagination if there are more results</returns>
        public async Task<ItemsWithCursor<WrappedTimeSeriesId>> ListMembersAsync(ListSubscriptionMembers query, CancellationToken token = default)
        {
            var req = Subscriptions.listMembers(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List subscriptions in the project, with pagination.
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>A list of subscriptions, and a cursor for pagination if there are more results</returns>
        public async Task<ItemsWithCursor<Subscription>> ListAsync(ListSubscriptions query, CancellationToken token = default)
        {
            var req = Subscriptions.list(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of subscriptions by their external ID.
        /// </summary>
        /// <param name="items">Subscriptions to retrieve</param>
        /// <param name="ignoreUnknownIds">True to ignore IDs not present in CDF,
        /// if this is false and subscriptions are missing, the request will fail.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved subscriptions</returns>
        public async Task<IEnumerable<Subscription>> RetrieveAsync(IEnumerable<string> items, bool ignoreUnknownIds = false, CancellationToken token = default)
        {
            var req = Subscriptions.retrieve(items, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a list of subscriptions.
        /// </summary>
        /// <param name="updates">Subscription updates</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Updated subscriptions</returns>
        public async Task<IEnumerable<Subscription>> UpdateAsync(IEnumerable<UpdateItem<SubscriptionUpdate>> updates, CancellationToken token = default)
        {
            var req = Subscriptions.update(updates, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
