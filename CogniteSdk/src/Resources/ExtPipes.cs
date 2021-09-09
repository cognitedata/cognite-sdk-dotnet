﻿using Oryx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all extraction pipeline methods.
    /// </summary>
    public class ExtPipesResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal ExtPipesResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Returns list of extraction pipelines matching query.
        /// </summary>
        /// <param name="query">Query filter to use</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of extraction pipelines matching given filters, and optional cursor</returns>
        public async Task<ItemsWithCursor<ExtPipe>> ListAsync(ExtPipeQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.ExtPipes.list(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create extraction pipelines
        /// </summary>
        /// <param name="extPipes">Extraction pipelines to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Create extraction pipelines</returns>
        public async Task<IEnumerable<ExtPipe>> CreateAsync(IEnumerable<ExtPipeCreate> extPipes, CancellationToken token = default)
        {
            if (extPipes is null) throw new ArgumentNullException(nameof(extPipes));

            var req = Oryx.Cognite.ExtPipes.create(extPipes);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete extraction pipelines, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="query">Delete query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task<EmptyResponse> DeleteAsync(ExtPipeDelete query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.ExtPipes.delete(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }


        /// <summary>
        /// Delete extraction pipelines, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="ids">Ids to delete</param>
        /// <param name="ignoreUnknownIds">True to ignore missing ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));

            var req = new ExtPipeDelete
            {
                IgnoreUnknownIds = ignoreUnknownIds,
                Items = ids
            };
            return await DeleteAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete extraction pipelines, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="internalIds">Internal ids to delete</param>
        /// <param name="ignoreUnknownIds">True to ignore missing ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (internalIds is null) throw new ArgumentNullException(nameof(internalIds));

            var req = new ExtPipeDelete
            {
                IgnoreUnknownIds = ignoreUnknownIds,
                Items = internalIds.Select(Identity.Create)
            };
            return await DeleteAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete extraction pipelines, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="externalIds">External ids to delete</param>
        /// <param name="ignoreUnknownIds">True to ignore missing ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (externalIds is null) throw new ArgumentNullException(nameof(externalIds));

            var req = new ExtPipeDelete
            {
                IgnoreUnknownIds = ignoreUnknownIds,
                Items = externalIds.Select(Identity.Create)
            };
            return await DeleteAsync(req, token).ConfigureAwait(false);
        }
        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieve a list of extraction pipelines, optionally ignoring unknown ids
        /// </summary>
        /// <param name="ids">Ids to retrieve</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved extraction pipelines</returns>
        public async Task<IEnumerable<ExtPipe>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));

            var req = Oryx.Cognite.ExtPipes.retrieve(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of extraction pipelines, optionally ignoring unknown ids
        /// </summary>
        /// <param name="externalIds">External ids to retrieve</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved extraction pipelines</returns>
        public async Task<IEnumerable<ExtPipe>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (externalIds is null) throw new ArgumentNullException(nameof(externalIds));

            var req = externalIds.Select(Identity.Create);
            return await RetrieveAsync(req, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of extraction pipelines, optionally ignoring unknown ids
        /// </summary>
        /// <param name="internalIds">Internal ids to retrieve</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved extraction pipelines</returns>
        public async Task<IEnumerable<ExtPipe>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (internalIds is null) throw new ArgumentNullException(nameof(internalIds));

            var req = internalIds.Select(Identity.Create);
            return await RetrieveAsync(req, ignoreUnknownIds, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Update a list of extraction pipelines
        /// </summary>
        /// <param name="query">Extraction pipeline updates</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task<IEnumerable<ExtPipe>> UpdateAsync(IEnumerable<UpdateItem<ExtPipeUpdate>> query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.ExtPipes.update(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// List extraction pipeline runs matching filter
        /// </summary>
        /// <param name="query">Extraction pipeline run query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of extraction pipeline runs with optional cursor</returns>
        public async Task<ItemsWithCursor<ExtPipeRun>> ListRunsAsync(ExtPipeRunQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.ExtPipes.listRuns(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a list of extraction pipeline runs
        /// </summary>
        /// <param name="items">Extraction pipeline runs to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of created extraction pipeline runs</returns>
        public async Task<IEnumerable<ExtPipeRun>> CreateRunsAsync(IEnumerable<ExtPipeRunCreate> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.ExtPipes.createRuns(items);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}