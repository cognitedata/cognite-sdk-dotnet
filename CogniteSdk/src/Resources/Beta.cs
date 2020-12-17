// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

using HttpContext = Oryx.Context<Microsoft.FSharp.Core.Unit>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all Playground resources.
    /// </summary>
    public class BetaResource : Resource
    {
        /// <summary>
        /// Client Relationships extension methods
        /// </summary>
        public Beta.RelationshipResource Relationships  { get; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal BetaResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
            Relationships = new Beta.RelationshipResource(authHandler, ctx);
        }
    }
}
