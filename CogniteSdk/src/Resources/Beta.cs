// Copyright 2020-2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Resources.Beta;
using Microsoft.FSharp.Core;
using Oryx;
using BetaDataModelsResource = CogniteSdk.Resources.Beta.DataModelsResource;
// Avoid name collision with the non-beta resources
using BetaDataPointsResource = CogniteSdk.Resources.Beta.DataPointsResource;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all Playground resources.
    /// </summary>
    public class BetaResource : Resource
    {
        /// <summary>
        /// Timeseries subscriptions
        /// </summary>
        public SubscriptionsResource Subscriptions { get; }

        /// <summary>
        /// Resource for Stream Records.
        /// </summary>
        public StreamRecordsResource StreamRecords { get; }

        /// <summary>
        /// Beta time series data points
        /// </summary>
        public BetaDataPointsResource DataPoints { get; }

        /// <summary>
        /// Beta data modeling operations
        /// </summary>
        public BetaDataModelsResource DataModels { get; }

        /// <summary>
        /// Core data model state sets, describing the possible states of state time series.
        /// </summary>
        public StateSetsResource StateSets { get; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal BetaResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
            Subscriptions = new SubscriptionsResource(authHandler, ctx);
            StreamRecords = new StreamRecordsResource(authHandler, ctx);
            DataPoints = new BetaDataPointsResource(authHandler, ctx);
            DataModels = new BetaDataModelsResource(authHandler, ctx);
            StateSets = new StateSetsResource(authHandler, ctx);
        }
    }
}
