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
    /// For internal use. Contains all data points methods.
    /// </summary>
    public class TimeSeriesResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal TimeSeriesResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Unit, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Exception, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, Task<Unit>>, Task<Unit>>>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Asynchronously retrieve list of time series (with or without meta-data) matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <typeparam name="T">Type of asset to return, e.g Assset or AssetWithoutMetadata.</typeparam>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public async Task<IItemsWithCursor<T>> ListAsync<T>(TimeSeriesQuery query, CancellationToken token = default) where T : TimeSeries
        {
            var req = Oryx.Cognite.TimeSeries.list<T>(query, _ctx);
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
            return (ItemsWithCursor<TimeSeries>) await ListAsync<TimeSeries>(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve the number of time series matching query.
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

            var req = Oryx.Cognite.TimeSeries.aggregate(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously create one or more time series.
        /// </summary>
        /// <param name="timeseries">Time series to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TimeSeries>> CreateAsync(IEnumerable<TimeSeriesCreate> timeseries, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.create<TimeSeries>(timeseries, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about an time series like object given a time series id.
        /// </summary>
        /// <param name="tsId">The id of the time series to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <typeparam name="T">Type of time series to return, e.g TimeSeries or TimeSeriesWithoutMetadata.</typeparam>
        /// <returns>Time series with the given id.</returns>
        public async Task<T> GetAsync<T>(long tsId, CancellationToken token = default) where T : TimeSeries
        {
            var req = Oryx.Cognite.TimeSeries.get<T>(tsId, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about an time series given a time series id.
        /// </summary>
        /// <param name="tsId">The id of the time series to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Time series with the given id.</returns>
        public async Task<TimeSeries> GetAsync(long tsId, CancellationToken token = default)
        {
            return await GetAsync<TimeSeries>(tsId, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously delete multiple time series in the same project.
        /// </summary>
        /// <param name="query">The list of time series to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(TimeSeriesDelete query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.TimeSeries.delete(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously delete multiple time series in the same project.
        /// </summary>
        /// <param name="internalIds">The list of time series internal ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var query = new TimeSeriesDelete { Items=internalIds.Select(Identity.Create) };
            var req = Oryx.Cognite.TimeSeries.delete(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously delete multiple time series in the same project.
        /// </summary>
        /// <param name="externalIds">The list of timeseries external ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var query = new TimeSeriesDelete { Items=externalIds.Select(Identity.Create) };
            var req = Oryx.Cognite.TimeSeries.delete(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <typeparam name="T">Type of time series to return, e.g TimeSeries or TimeSeriesWithoutMetadata.</typeparam>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<T>> RetrieveAsync<T>(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : TimeSeries
        {
            var req = Oryx.Cognite.TimeSeries.retrieve<T>(ids, ignoreUnknownIds, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of time series identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested time series.</returns>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return await RetrieveAsync<TimeSeries>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested time series.</returns>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve information about multiple time series in the same project. A maximum of 1000 time
        /// series IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of time series internal ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested time series.</returns>
        public async Task<IEnumerable<TimeSeries>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of time series (with or without meta-data) matching the given criteria. This
        /// operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <typeparam name="T">Type of time series to return, e.g TimeSeries or TimeSeriesWithoutMetadata.</typeparam>
        /// <returns>Sequence of time series matching given criteria.</returns>
        public async Task<IEnumerable<T>> SearchAsync<T>(TimeSeriesSearch query, CancellationToken token = default) where T : TimeSeries
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.search<T>(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of time series matching the given criteria. This operation does not support
        /// pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of time series matching given criteria.</returns>
        public async Task<IEnumerable<TimeSeries>> SearchAsync(TimeSeriesSearch query, CancellationToken token = default)
        {
            return await SearchAsync<TimeSeries>(query, token).ConfigureAwait(false);
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

            var req = Oryx.Cognite.TimeSeries.update<TimeSeries>(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public async Task<IEnumerable<DataPointsSyntheticItem>> SyntheticQueryAsync(TimeSeriesSyntheticQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.TimeSeries.syntheticQuery(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}
