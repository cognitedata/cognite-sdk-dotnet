// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Beta;

using Oryx;
using Oryx.Cognite.Beta;
using Oryx.Pipeline;

using Microsoft.FSharp.Core;

namespace CogniteSdk.Resources.Beta
{
    /// <summary>
    /// Resource for data models
    /// </summary>
    public class DataModelsResource : Resource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DataModelsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create a list of spaces
        /// </summary>
        /// <param name="spaces">Spaces to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created spaces</returns>
        public async Task<IEnumerable<Space>> CreateSpaces(IEnumerable<Space> spaces, CancellationToken token = default)
        {
            var req = DataModels.createSpaces(spaces, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of spaces
        /// </summary>
        /// <param name="ids">ExternalIds of spaces to delete</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task DeleteSpaces(IEnumerable<string> ids, CancellationToken token = default)
        {
            var req = DataModels.deleteSpaces(ids, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List all spaces
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>All spaces in project</returns>
        public async Task<IEnumerable<Space>> ListSpaces(CancellationToken token = default)
        {
            var req = DataModels.listSpaces(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve spaces by externalId
        /// </summary>
        /// <param name="ids">ExternalIds of spaces to retrieve</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved spaces</returns>
        public async Task<IEnumerable<Space>> RetrieveSpaces(IEnumerable<string> ids, CancellationToken token = default)
        {
            var req = DataModels.retrieveSpaces(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a list of data models
        /// </summary>
        /// <param name="models">Models to create</param>
        /// <param name="space">Space to create models in</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created models</returns>
        public async Task<IEnumerable<Model>> ApplyModels(IEnumerable<ModelCreate> models, string space, CancellationToken token = default)
        {
            var req = DataModels.applyModels(models, space, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of data models
        /// </summary>
        /// <param name="ids">Models to delete</param>
        /// <param name="space">Space to delete models in</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task DeleteModels(IEnumerable<string> ids, string space, CancellationToken token = default)
        {
            var req = DataModels.deleteModels(ids, space, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List all data models in a space
        /// </summary>
        /// <param name="space">Space to list models in</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>All data models in space</returns>
        public async Task<IEnumerable<Model>> ListModels(string space, CancellationToken token = default)
        {
            var req = DataModels.listModels(space, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of data models from space
        /// </summary>
        /// <param name="ids">ExternalIds of spaces to retrieve</param>
        /// <param name="space">Space to retrieve models from</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved data models</returns>
        public async Task<IEnumerable<Model>> RetrieveModels(IEnumerable<string> ids, string space, CancellationToken token = default)
        {
            var req = DataModels.retrieveModels(ids, space, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Ingest a list of nodes, this is an upsert.
        /// </summary>
        /// <typeparam name="T">Type of node to ingest, must match model being ingested into</typeparam>
        /// <param name="request">Nodes to ingest</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created or updated nodes</returns>
        public async Task<IEnumerable<T>> IngestNodes<T>(NodeIngestRequest<T> request, CancellationToken token = default) where T : BaseNode
        {
            var req = DataModels.ingestNodes(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of nodes.
        /// </summary>
        /// <param name="ids">Node externalIds to delete</param>
        /// <param name="space">Space to delete nodes from</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task DeleteNodes(IEnumerable<string> ids, string space, CancellationToken token = default)
        {
            var req = DataModels.deleteNodes(ids, space, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List nodes with given filter
        /// </summary>
        /// <typeparam name="T">Retrieved node type</typeparam>
        /// <param name="query">Query describing what nodes to fetch, sort, etc.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved nodes with cursor for pagination</returns>
        public async Task<NodeListResponse<T>> FilterNodes<T>(NodeFilterQuery query, CancellationToken token = default) where T : BaseNode
        {
            var req = DataModels.filterNodes<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Search nodes with given query
        /// </summary>
        /// <typeparam name="T">Retrieved node type</typeparam>
        /// <param name="query">Search query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Up to 1000 nodes</returns>
        public async Task<NodeListResponse<T>> SearchNodes<T>(NodeSearchQuery query, CancellationToken token = default) where T : BaseNode
        {
            var req = DataModels.searchNodes<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of nodes by externalId
        /// </summary>
        /// <typeparam name="T">Retrieved node type</typeparam>
        /// <param name="query">Model identifier and list of node external ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved list of nodes</returns>
        public async Task<NodeListResponse<T>> RetrieveNodes<T>(RetrieveNodesRequest query, CancellationToken token = default) where T : BaseNode
        {
            var req = DataModels.retrieveNodes<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Ingest a list of edges, this is an upsert.
        /// </summary>
        /// <typeparam name="T">Type of node to ingest, must match model being ingested into</typeparam>
        /// <param name="request">Edges to ingest</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Ingested edges</returns>
        public async Task<IEnumerable<T>> IngestEdges<T>(EdgeIngestRequest<T> request, CancellationToken token = default) where T : BaseEdge
        {
            var req = DataModels.ingestEdges<T>(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of edges by edge externalId.
        /// </summary>
        /// <param name="ids">ExternalIds of edges to delete</param>
        /// <param name="space">Space to delete edges in</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task DeleteEdges(IEnumerable<string> ids, string space, CancellationToken token = default)
        {
            var req = DataModels.deleteEdges(ids, space, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List edges with given filter
        /// </summary>
        /// <typeparam name="T">Retrieved edge type</typeparam>
        /// <param name="query">Query describing what dges to fetch, sort, etc.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved edges with cursor for pagination</returns>
        public async Task<EdgeListResponse<T>> FilterEdges<T>(NodeFilterQuery query, CancellationToken token = default) where T : BaseEdge
        {
            var req = DataModels.filterEdges<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Search edges with given query
        /// </summary>
        /// <typeparam name="T">Retrieved edge type</typeparam>
        /// <param name="query">Search query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Up to 1000 edges</returns>
        public async Task<EdgeListResponse<T>> SearchEdges<T>(NodeSearchQuery query, CancellationToken token = default) where T : BaseEdge
        {
            var req = DataModels.searchEdges<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of edges by externalId
        /// </summary>
        /// <typeparam name="T">Retrieved edge type</typeparam>
        /// <param name="query">Model identifier and list of edge external ids</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved list of edges</returns>
        public async Task<EdgeListResponse<T>> RetrieveEdges<T>(RetrieveNodesRequest query, CancellationToken token = default) where T : BaseEdge
        {
            var req = DataModels.retrieveEdges<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Perform a graph query. See API docs to create a suitable result type
        /// for your query.
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="query">Query to execute</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Graph query result</returns>
        public async Task<T> GraphQuery<T>(GraphQuery query, CancellationToken token = default)
        {
            var req = DataModels.graphQuery<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
