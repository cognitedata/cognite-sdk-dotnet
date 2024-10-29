// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Resources.Alpha;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all alpha resources.
    /// </summary>
    public class AlphaResource : Resource
    {
        /// <summary>
        /// Client Simulators extension methods
        /// </summary>
        public SimulatorsResource Simulators { get; }

        /// <summary>
        /// Resource for Industrial Log Analytics (ILA)
        /// </summary>
        public LogAnalyticsResource LogAnalytics { get; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal AlphaResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
            Simulators = new SimulatorsResource(authHandler, ctx);
            LogAnalytics = new LogAnalyticsResource(authHandler, ctx);
        }
    }
}
