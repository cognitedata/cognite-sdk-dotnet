// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all Playground resources.
    /// </summary>
    public class PlaygroundResource : Resource
    {
        /// <summary>
        /// Client Relationships extension methods
        /// </summary>
        public RelationshipResource Relationships  { get; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authntication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal PlaygroundResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
            Relationships = new RelationshipResource(authHandler, ctx);
        }
    }
}