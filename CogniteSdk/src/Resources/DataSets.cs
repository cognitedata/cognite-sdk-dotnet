// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="ctx">Context to use for the request.</param>
        internal DataSetsResource(Func<CancellationToken, Task<string>> authHandler, Context ctx) : base(authHandler, ctx)
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

            var req = DataSets.create(dataSets);
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

            var req = DataSets.list<T>(query);
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
    }
}
