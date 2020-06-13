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
        /// Client Assets extension methods
        /// </summary>
        public Playground.AssetsResource Assets { get; }

        /// <summary>
        /// Client Functions extension methods
        /// </summary>
        public Playground.FunctionResource Functions { get; set; }

        /// <summary>
        /// Client Relationships extension methods
        /// </summary>
        public Playground.RelationshipResource Relationships  { get; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal PlaygroundResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
            Assets = new Playground.AssetsResource(authHandler, ctx);
            Functions = new Playground.FunctionResource(authHandler, ctx);
            Relationships = new Playground.RelationshipResource(authHandler, ctx);
        }
    }
}