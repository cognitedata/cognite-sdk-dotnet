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
    }
}
