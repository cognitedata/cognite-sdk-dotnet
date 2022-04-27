// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

using Oryx;
using Oryx.Cognite;
using Oryx.Pipeline;

using Microsoft.FSharp.Core;

namespace CogniteSdk.Resources.Playground
{
    /// <summary>
    /// Resource for extractor configuration
    /// </summary>
    public class ExtPipeConfigs : Resource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExtPipeConfigs(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create a new config revision
        /// </summary>
        /// <param name="config">Configuration object to add</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created config objects</returns>
        public async Task<ExtPipeConfig> Create(ExtPipeConfigCreate config, CancellationToken token = default)
        {
            var req = ExtPipes.createConfig(config, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Get current config revision
        /// </summary>
        /// <param name="extPipeId">Extraction pipeline id</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved config object</returns>
        public async Task<ExtPipeConfig> GetCurrentConfig(string extPipeId, CancellationToken token = default)
        {
            var req = ExtPipes.getCurrentConfig(extPipeId, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a specific config revision
        /// </summary>
        /// <param name="extPipeId">Extraction pipeline id</param>
        /// <param name="revision">Revision to fetch</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved config object</returns>
        public async Task<ExtPipeConfig> GetConfigRevision(string extPipeId, int revision, CancellationToken token = default)
        {
            var req = ExtPipes.getConfigRevision(extPipeId, revision, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List config revisions for the specified extraction pipeline, with optional limit and cursor.
        /// </summary>
        /// <param name="query">Query with optional limit and cursor</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved list of configuration objects</returns>
        public async Task<ItemsWithCursor<ExtPipeConfig>> ListConfigRevisions(ListConfigQuery query, CancellationToken token = default)
        {
            var req = ExtPipes.listConfigRevisions(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Revert to a previous config revision. This adds a new revision equal to the old one.
        /// </summary>
        /// <param name="extPipeId">Extraction pipeline id</param>
        /// <param name="revision">Revision to revert to</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>New revision</returns>
        public async Task<ExtPipeConfig> RevertConfigRevision(string extPipeId, int revision, CancellationToken token = default)
        {
            var req = ExtPipes.revertConfigRevision(extPipeId, revision, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
