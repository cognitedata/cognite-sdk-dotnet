// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all Playground resources.
    /// </summary>
    public class PlaygroundResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Client Relationships extension methods
        /// </summary>
        public RelationshipResource Relationships  { get; }

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal PlaygroundResource(HttpContext ctx)
        {
            _ctx = ctx;
            Relationships = new RelationshipResource(ctx);
        }
    }
}