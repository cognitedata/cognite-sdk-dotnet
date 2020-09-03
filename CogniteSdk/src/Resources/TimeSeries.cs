// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all data points methods.
    /// </summary>
    public class TimeSeriesResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal TimeSeriesResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Asynchronously retrieve list of time series (with or without meta-data) matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public async Task<IItemsWithCursor<T>> ListAsync<T>(TimeSeriesQuery query, CancellationToken token = default) where T : TimeSeries
        {
            var req = Oryx.Cognite.TimeSeries.list<T,  ItemsWithCursor<T>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<TimeSeries>> ListAsync(TimeSeriesQuery query, CancellationToken token = default)
        {
            return (ItemsWithCursor<TimeSeries>) await ListAsync<TimeSeries>(query, token);
        }

        /// <summary>
        /// Retrieve list of time series like objects matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public IItemsWithCursor<T> List<T>(TimeSeriesQuery query, CancellationToken token = default) where T : TimeSeries
        {
            return ListAsync<T>(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieve list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public IItemsWithCursor<TimeSeries> List(TimeSeriesQuery query, CancellationToken token = default)
        {
            return ListAsync<TimeSeries>(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously retrieve the number of timeseries matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of timeseries matching given filters</returns>
        public async Task<int> AggregateAsync(TimeSeriesQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.aggregate<Int32>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the number of timeseries matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of timeseries matching given filters</returns>
        public int Aggregate(TimeSeriesQuery query, CancellationToken token = default)
        {
            return AggregateAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously create one or more time series.
        /// </summary>
        /// <param name="timeseries">Time series to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TimeSeries>> CreateAsync(IEnumerable<TimeSeriesCreate> timeseries, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.create<TimeSeries, IEnumerable<TimeSeries>>(timeseries);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create one or more time series.
        /// </summary>
        /// <param name="timeseries">Time series to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public IEnumerable<TimeSeries> Create(IEnumerable<TimeSeriesCreate> timeseries, CancellationToken token = default)
        {
            return CreateAsync(timeseries, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously delete multiple times series in the same project.
        /// </summary>
        /// <param name="query">The list of timeseries to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(TimeSeriesDelete query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple times series in the same project.
        /// </summary>
        /// <param name="query">The list of timeseries to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(TimeSeriesDelete query, CancellationToken token = default)
        {
            return DeleteAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously delete multiple times eries in the same project.
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
        /// <param name="internalIds">The list of timeseries internal ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            return DeleteAsync(internalIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously delete multiple times eries in the same project.
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
        /// Delete multiple times series in the same project.
        /// </summary>
        /// <param name="externalIds">The list of timeseries external ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public EmptyResponse Delete(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            return DeleteAsync(externalIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<T>> RetrieveAsync<T>(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : TimeSeries
        {
            var req = Oryx.Cognite.TimeSeries.retrieve<T, IEnumerable<T>>(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return await RetrieveAsync<TimeSeries>(ids, ignoreUnknownIds, token);
        }

        /// <summary>
        /// Retrieve information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<T> Retrieve<T>(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default) where T :  TimeSeries
        {
            return RetrieveAsync<T>(ids, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieve information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<TimeSeries> Retrieve(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync<TimeSeries>(ids, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve information about multiple time series like objects in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<T> Retrieve<T>(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : TimeSeries
        {
            var ids = internalIds.Select(Identity.Create);
            return RetrieveAsync<T>(ids, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieve information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<TimeSeries> Retrieve(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(internalIds, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve information about multiple time series like objects in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<T> Retrieve<T>(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : TimeSeries
        {
            var ids = externalIds.Select(Identity.Create);
            return RetrieveAsync<T>(ids, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieve information about multiple time series in the same project. A maximum of 1000 time series IDs may
        /// be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public IEnumerable<TimeSeries> Retrieve(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return RetrieveAsync(externalIds, ignoreUnknownIds, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously retrieve a list of time series (with or without meta-data) matching the given criteria. This
        /// operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<T>> SearchAsync<T>(TimeSeriesSearch query, CancellationToken token = default) where T : TimeSeries
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.search<T, IEnumerable<T>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of time series matching the given criteria. This operation does not support
        /// pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<TimeSeries>> SearchAsync(TimeSeriesSearch query, CancellationToken token = default)
        {
            return await SearchAsync<TimeSeries>(query, token);
        }

        /// <summary>
        /// Retrieve a list of time series like objects matching the given criteria. This operation does not support
        /// pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public IEnumerable<T> Search<T>(TimeSeriesSearch query, CancellationToken token = default) where T : TimeSeries
        {
            return SearchAsync<T>(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieve a list of time series matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public IEnumerable<TimeSeries> Search(TimeSeriesSearch query, CancellationToken token = default)
        {
            return SearchAsync<TimeSeries>(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously update multiple time series within the same project. This operation supports partial updates, meaning that
        /// fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of timeseries to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated timeseries.</returns>
        public async Task<IEnumerable<TimeSeries>> UpdateAsync(IEnumerable<TimeSeriesUpdateItem> query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.update<TimeSeries, IEnumerable<TimeSeries>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Update multiple time series within the same project. This operation supports partial updates, meaning that
        /// fields omitted from the requests are not changed
        /// </summary>
        /// <param name="query">The list of timeseries to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated timeseries.</returns>
        public IEnumerable<TimeSeries> Update(IEnumerable<TimeSeriesUpdateItem> query, CancellationToken token = default )
        {
            return UpdateAsync(query, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously retrieve a list of time series matching query.
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

        /// <summary>
        /// Retrieve a list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public IEnumerable<DataPointsSyntheticItem> SyntheticQuery(TimeSeriesSyntheticQuery query, CancellationToken token = default)
        {
            return SyntheticQueryAsync(query, token).GetAwaiter().GetResult();
        }
    }
}
