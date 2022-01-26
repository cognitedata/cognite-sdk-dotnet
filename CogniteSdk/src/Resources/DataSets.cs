// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all data set methods.
    /// </summary>
    public class DataSetsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal DataSetsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Unit, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, FSharpFunc<Exception, Task<Unit>>>, FSharpFunc<FSharpFunc<HttpContext, Task<Unit>>, Task<Unit>>>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Asynchronously create data sets.
        /// </summary>
        /// <param name="dataSets">Data sets to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created dataSets.</returns>
        public async Task<IEnumerable<DataSet>> CreateAsync(IEnumerable<DataSetCreate> dataSets, CancellationToken token = default)
        {
            if (dataSets is null) throw new ArgumentNullException(nameof(dataSets));

            var req = DataSets.create(dataSets, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of data set like objects matching the given filter.
        /// </summary>
        /// <typeparam name="T">Type of data set to return, e.g. DataSet or DataSetWithoutMetadata.</typeparam>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of data sets matching given filters and optional cursor</returns>
        public async Task<IItemsWithCursor<T>> ListAsync<T>(DataSetQuery query, CancellationToken token = default) where T : DataSet
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = DataSets.list<T>(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve a list of data sets matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of data sets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<DataSet>> ListAsync(DataSetQuery query, CancellationToken token = default)
        {
            return (ItemsWithCursor<DataSet>)await ListAsync<DataSet>(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve number of data sets matching query.
        /// </summary>
        /// <param name="query">The query filter to use</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Number of data sets matching given filters</returns>
        public async Task<int> AggregateAsync(DataSetQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = DataSets.aggregate(query, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Retrieve overloads
        /// <summary>
        /// Asynchronously retrieves information about multiple data set like objects in the same project. A maximum of
        /// 1000 data set IDs may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of data set identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <typeparam name="T">Type of data set to return, e.g DataSet or DataSetWithoutMetadata.</typeparam>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested data sets.</returns>
        public async Task<IEnumerable<T>> RetrieveAsync<T>(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default) where T : DataSet
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = DataSets.retrieve<T>(ids, ignoreUnknownIds, _ctx);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple data sets in the same project. A maximum of 1000 data set IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="ids">The list of data set identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested data sets.</returns>
        public async Task<IEnumerable<DataSet>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            return await RetrieveAsync<DataSet>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple data sets in the same project. A maximum of 1000 data set IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="internalIds">The list of data set internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested data sets.</returns>
        public async Task<IEnumerable<DataSet>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync<DataSet>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple data sets in the same project. A maximum of 1000 data set IDs
        /// may be listed per request and all of them must be unique.
        /// </summary>
        /// <param name="externalIds">The list of data set external identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested data sets.</returns>
        public async Task<IEnumerable<DataSet>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync<DataSet>(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Asynchronously update one or more data sets. Supports partial updates, meaning that fields omitted from the
        /// requests are not changed
        /// </summary>
        /// <param name="query">The list of data sets to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of updated data sets.</returns>
        public async Task<IEnumerable<DataSet>> UpdateAsync(IEnumerable<DataSetUpdateItem> query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = DataSets.update<DataSet>(query, _ctx);
            var ret = await RunAsync(req, token).ConfigureAwait(false);
            return ret;
        }
    }
}
