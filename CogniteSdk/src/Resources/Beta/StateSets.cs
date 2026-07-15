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
    /// Resource for core data model state sets, describing the possible states of state time series.
    /// State sets are currently a beta feature.
    /// </summary>
    public class StateSetsResource : Resource
    {
        /// <summary>
        /// View backing state sets in the core data model.
        /// </summary>
        public static readonly ViewIdentifier View = new ViewIdentifier("cdf_cdm", "CogniteStateSet", "v1");

        /// <summary>
        /// Constructor
        /// </summary>
        public StateSetsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create or update a list of state sets.
        /// </summary>
        /// <param name="items">State sets to upsert.</param>
        /// <param name="options">Optional upsert options.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>The upserted state set instances.</returns>
        public async Task<IEnumerable<SlimInstance>> UpsertAsync(
            IEnumerable<SourcedNodeWrite<CogniteStateSet>> items,
            UpsertOptions options = null,
            CancellationToken token = default)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
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
                        new InstanceData<CogniteStateSet>
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
        /// Retrieve a list of state sets by instance ID.
        /// </summary>
        /// <param name="ids">Instance IDs to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>The retrieved state set instances.</returns>
        public async Task<IEnumerable<SourcedNode<CogniteStateSet>>> RetrieveAsync(
            IEnumerable<InstanceIdentifierWithType> ids,
            CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var request = new InstancesRetrieve
            {
                Items = ids,
                Sources = new[] { new InstanceSource { Source = View } }
            };

            var req = Oryx.Cognite.Beta.DataModels.retrieveInstances<Dictionary<string, Dictionary<string, CogniteStateSet>>>(request, GetContext(token));
            var results = await RunAsync(req).ConfigureAwait(false);

            return results.Items.Select(r => new SourcedNode<CogniteStateSet>
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
