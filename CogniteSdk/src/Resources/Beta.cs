// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Resources.Beta;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all Playground resources.
    /// </summary>
    public class BetaResource : Resource
    {
        /// <summary>
        /// Client Templates extension methods
        /// </summary>
        public TemplatesResource Templates { get; }

        /// <summary>
        /// Timeseries subscriptions
        /// </summary>
        public SubscriptionsResource Subscriptions { get; }

        /// <summary>
        /// Resource for Stream Records.
        /// </summary>
        public StreamRecordsResource StreamRecords { get; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal BetaResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
            Templates = new TemplatesResource(authHandler, ctx);
            Subscriptions = new SubscriptionsResource(authHandler, ctx);
            StreamRecords = new StreamRecordsResource(authHandler, ctx);
        }
    }
}
