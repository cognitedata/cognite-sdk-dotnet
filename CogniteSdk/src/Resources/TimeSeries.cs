// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static Oryx.Cognite.HandlerModule;
using CogniteSdk;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;
using System;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all data points methods.
    /// </summary>
    public class TimeSeriesResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal TimeSeriesResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<TimeSeries>> ListAsync(TimeSeriesQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.list<ItemsWithCursor<TimeSeries>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves number of timeseries matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of timeseries matching given filters</returns>
        public async Task<Int32> AggregateAsync(TimeSeriesQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.aggregate<Int32>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create 1 or more time series.
        /// </summary>
        /// <param name="timeseries">Time series to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TimeSeries>> CreateAsync(IEnumerable<TimeSeriesCreate> timeseries, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.create<IEnumerable<TimeSeries>>(timeseries);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple times eries in the same project.
        /// </summary>
        /// <param name="query">The list of timeseries to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(TimeSeriesDelete query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple times eries in the same project.
        /// </summary>
        /// <param name="internalIds">The list of timeseries internal ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var query = new TimeSeriesDelete { Items=internalIds.Select(Identity.Create) };
            var req = Oryx.Cognite.TimeSeries.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple times eries in the same project.
        /// </summary>
        /// <param name="externalIds">The list of timeseries external ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var query = new TimeSeriesDelete { Items=externalIds.Select(Identity.Create) };
            var req = Oryx.Cognite.TimeSeries.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.retrieve<IEnumerable<TimeSeries>>(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            var req = Oryx.Cognite.TimeSeries.retrieve<IEnumerable<TimeSeries>>(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = externalIds.Select(Identity.Create);
            var req = Oryx.Cognite.TimeSeries.retrieve<IEnumerable<TimeSeries>>(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of time series matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<TimeSeries>> SearchAsync (TimeSeriesSearch query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.search<IEnumerable<TimeSeries>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates multiple time series within the same project. This operation supports partial updates, meaning that
        /// fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of timeseries to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated timeseries.</returns>
        public async Task<IEnumerable<TimeSeries>> UpdateAsync (IEnumerable<TimeSeriesUpdateItem> query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.update<IEnumerable<TimeSeries>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<IEnumerable<DataPointsSyntheticItem>> SyntheticQueryAsync(TimeSeriesSyntheticQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.syntheticQuery<IEnumerable<DataPointsSyntheticItem>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}
