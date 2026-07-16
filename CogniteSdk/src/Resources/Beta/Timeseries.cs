// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources.Beta
{
    /// <summary>
    /// Resource for core data model time series.
    /// </summary>
    public class TimeSeriesResource : Resource
    {
        /// <summary>
        /// View backing time series in the core data model.
        /// </summary>
        public static readonly ViewIdentifier View = new ViewIdentifier("cdf_cdm", "CogniteTimeSeries", "v1");

        /// <summary>
        /// Will only be instantiated by the client
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal TimeSeriesResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create or update a list of time series.
        /// </summary>
        /// <param name="items">Time series to upsert.</param>
        /// <param name="options">Optional upsert options.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>The upserted time series instances.</returns>
        public async Task<IEnumerable<SlimInstance>> UpsertAsync(
            IEnumerable<SourcedNodeWrite<CogniteTimeSeriesBase>> items,
            UpsertOptions options = null,
            CancellationToken token = default)
        {
            if (items is null)
            {
                return Enumerable.Empty<SlimInstance>();
            }

            var opts = options ?? new UpsertOptions();
            var request = new InstanceWriteRequest
            {
                AutoCreateDirectRelations = opts.AutoCreateDirectRelations,
                AutoCreateEndNodes = opts.AutoCreateEndNodes,
                AutoCreateStartNodes = opts.AutoCreateStartNodes,
                SkipOnVersionConflict = opts.SkipOnVersionConflict,
                Replace = opts.Replace,
                Items = items.Select(item => (BaseInstanceWrite)new NodeWrite
                {
                    ExistingVersion = item.ExistingVersion,
                    Space = item.Space,
                    ExternalId = item.ExternalId,
                    Type = item.Type,
                    Sources = new[]
                    {
                        new InstanceData<CogniteTimeSeriesBase>
                        {
                            Properties = item.Properties,
                            Source = View
                        }
                    }
                }).ToList()
            };

            var req = Oryx.Cognite.Beta.DataModels.upsertInstances(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of time series by instance ID.
        /// </summary>
        /// <param name="ids">Instance IDs to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>The retrieved time series instances.</returns>
        public async Task<IEnumerable<SourcedNode<CogniteTimeSeriesBase>>> RetrieveAsync(
            IEnumerable<InstanceIdentifierWithType> ids,
            CancellationToken token = default)
        {
            if (ids is null)
            {
                return Enumerable.Empty<SourcedNode<CogniteTimeSeriesBase>>();
            }

            var request = new InstancesRetrieve
            {
                Items = ids,
                Sources = new[] { new InstanceSource { Source = View } }
            };

            var req = Oryx.Cognite.Beta.DataModels.retrieveInstances<Dictionary<string, Dictionary<string, CogniteTimeSeriesBase>>>(request, GetContext(token));
            var results = await RunAsync(req).ConfigureAwait(false);

            return results.Items.Select(r => new SourcedNode<CogniteTimeSeriesBase>
            {
                Space = r.Space,
                ExternalId = r.ExternalId,
                Type = r.Type,
                Version = r.Version,
                CreatedTime = r.CreatedTime,
                LastUpdatedTime = r.LastUpdatedTime,
                DeletedTime = r.DeletedTime,
                Properties = DMHelpers.GetFromNestedDicts(r.Properties, View)
            }).ToList();
        }
    }
}
