// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Beta.DataModels;

using Oryx;
using Oryx.Cognite.Beta;

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
        public DataModelsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Update or insert a list of spaces
        /// </summary>
        /// <param name="spaces">Spaces to create or update</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created/updated spaces</returns>
        public async Task<IEnumerable<Space>> UpsertSpaces(IEnumerable<SpaceCreate> spaces, CancellationToken token = default)
        {
            var req = DataModels.upsertSpaces(spaces, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List spaces with pagination
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>All spaces in project</returns>
        public async Task<ItemsWithCursor<Space>> ListSpaces(SpaceQuery query, CancellationToken token = default)
        {
            var req = DataModels.listSpaces(query, GetContext(token));
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
        /// Delete a list of spaces
        /// </summary>
        /// <param name="ids">ExternalIds of spaces to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Deleted spaces</returns>
        public async Task<IEnumerable<string>> DeleteSpaces(IEnumerable<string> ids, CancellationToken token = default)
        {
            var req = DataModels.deleteSpaces(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Update or create a list of data models
        /// </summary>
        /// <param name="dataModels">Data models to create or update</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created data models</returns>
        public async Task<IEnumerable<DataModel>> UpsertDataModels(IEnumerable<DataModelCreate> dataModels, CancellationToken token = default)
        {
            var req = DataModels.upsertDataModels(dataModels, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List data models with optional pagination.
        /// </summary>
        /// <param name="query">Data model query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Data models with cursor for pagination</returns>
        public async Task<ItemsWithCursor<DataModel>> ListDataModels(DataModelQuery query, CancellationToken token = default)
        {
            var req = DataModels.listDataModels(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Filter data models with complex filters.
        /// </summary>
        /// <param name="filter">Data model filter</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Data models with cursor for pagination</returns>
        public async Task<ItemsWithCursor<DataModel>> FilterDataModels(DataModelFilter filter, CancellationToken token = default)
        {
            var req = DataModels.filterDataModels(filter, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve data models by ids.
        /// </summary>
        /// <param name="ids">Data model ids to retrieve</param>
        /// <param name="inlineViews">True to inline views in the retrieved data models</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved data models</returns>
        public async Task<IEnumerable<DataModel>> RetrieveDataModels(IEnumerable<FDMExternalId> ids, bool inlineViews, CancellationToken token = default)
        {
            var req = DataModels.retrieveDataModels(ids, inlineViews, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of data models.
        /// </summary>
        /// <param name="ids">Ids of data models to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Ids of deleted data models</returns>
        public async Task<IEnumerable<FDMExternalId>> DeleteDataModels(IEnumerable<FDMExternalId> ids, CancellationToken token = default)
        {
            var req = DataModels.deleteDataModels(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create or update views
        /// </summary>
        /// <param name="views">Views to create or update</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created or updated views</returns>
        public async Task<IEnumerable<View>> UpsertViews(IEnumerable<ViewCreate> views, CancellationToken token = default)
        {
            var req = DataModels.upsertViews(views, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List views with pagination
        /// </summary>
        /// <param name="query">View query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Views with cursor for pagination</returns>
        public async Task<ItemsWithCursor<View>> ListViews(ViewQuery query, CancellationToken token = default)
        {
            var req = DataModels.listViews(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve views by ids.
        /// </summary>
        /// <param name="ids">View ids to retrieve</param>
        /// <param name="includeInheritedProperties">True to include properties from inherited views, default true.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved views</returns>
        public async Task<IEnumerable<View>> RetrieveViews(IEnumerable<FDMExternalId> ids, bool includeInheritedProperties = true, CancellationToken token = default)
        {
            var req = DataModels.retrieveViews(ids, includeInheritedProperties, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete views by ids.
        /// </summary>
        /// <param name="ids">Ids of views to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Ids of deleted views</returns>
        public async Task<IEnumerable<FDMExternalId>> DeleteViews(IEnumerable<FDMExternalId> ids, CancellationToken token = default)
        {
            var req = DataModels.deleteViews(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create or update containers
        /// </summary>
        /// <param name="containers">Containers to create or update</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created or updated containers</returns>
        public async Task<IEnumerable<Container>> UpsertContainers(IEnumerable<ContainerCreate> containers, CancellationToken token = default)
        {
            var req = DataModels.upsertContainers(containers, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List containers with pagination.
        /// </summary>
        /// <param name="query">Containers query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Containers with cursor for pagination</returns>
        public async Task<ItemsWithCursor<Container>> ListContainers(ContainersQuery query, CancellationToken token = default)
        {
            var req = DataModels.listContainers(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve containers by ids.
        /// </summary>
        /// <param name="ids">Container ids to retrieve</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved containers</returns>
        public async Task<IEnumerable<Container>> RetrieveContainers(IEnumerable<ContainerId> ids, CancellationToken token = default)
        {
            var req = DataModels.retrieveContainers(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete containers by ids
        /// </summary>
        /// <param name="ids">Ids of containers to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Ids of deleted containers</returns>
        public async Task<IEnumerable<ContainerId>> DeleteContainers(IEnumerable<ContainerId> ids, CancellationToken token = default)
        {
            var req = DataModels.deleteContainers(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create or update instances
        /// </summary>
        /// <param name="request">Instances to create or update</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created or updated instances</returns>
        public async Task<IEnumerable<SlimInstance>> UpsertInstances(InstanceWriteRequest request, CancellationToken token = default)
        {
            var req = DataModels.upsertInstances(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Query instances with advanced filtering.
        /// </summary>
        /// <typeparam name="T">Type of instance data to retrieve</typeparam>
        /// <param name="filter">Instances filter</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved instances</returns>
        public async Task<InstancesFilterResponse<T>> FilterInstances<T>(InstancesFilter filter, CancellationToken token = default)
        {
            var req = DataModels.filterInstances<T>(filter, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve instances by ids
        /// </summary>
        /// <typeparam name="T">Type of instance data to retrieve</typeparam>
        /// <param name="query">Instances query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved instances</returns>
        public async Task<InstancesRetrieveResponse<T>> RetrieveInstances<T>(InstancesRetrieve query, CancellationToken token = default)
        {
            var req = DataModels.retrieveInstances<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Search instances with free text search.
        /// </summary>
        /// <typeparam name="T">Type of instance data to retrieve</typeparam>
        /// <param name="query">Instances search query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved instances</returns>
        public async Task<InstancesFilterResponse<T>> SearchInstances<T>(InstancesSearch query, CancellationToken token = default)
        {
            var req = DataModels.searchInstances<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Aggregate across instances.
        /// </summary>
        /// <param name="query">Instance aggregate query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Instance aggregates</returns>
        public async Task<InstancesAggregateResponse> AggregateInstances(InstancesAggregate query, CancellationToken token = default)
        {
            var req = DataModels.aggregateInstances(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete instances by ids.
        /// </summary>
        /// <param name="ids">Instance ids to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Ids of deleted instances</returns>
        public async Task<IEnumerable<InstanceIdentifier>> DeleteInstances(IEnumerable<InstanceIdentifier> ids, CancellationToken token = default)
        {
            var req = DataModels.deleteInstances(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Query fields from instances
        /// </summary>
        /// <typeparam name="T">Type of result data</typeparam>
        /// <param name="query">Instances query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Result of query</returns>
        public async Task<QueryResult<T>> QueryInstances<T>(Query query, CancellationToken token = default)
        {
            var req = DataModels.queryInstances<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetch changes to query results. This will always return a set of cursors which can be used to
        /// obtain the next set of changes.
        /// </summary>
        /// <typeparam name="T">Type of result data</typeparam>
        /// <param name="query">Sync query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Result of query</returns>
        public async Task<SyncResult<T>> SyncInstances<T>(SyncQuery query, CancellationToken token = default)
        {
            var req = DataModels.syncInstances<T>(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
