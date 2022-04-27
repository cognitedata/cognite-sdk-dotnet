using CogniteSdk.Beta;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite.Beta;
using Oryx.Pipeline;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CogniteSdk.Resources.Beta
{
    /// <summary>
    /// Resource for templates
    /// </summary>
    public class TemplatesResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        public TemplatesResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create a list of template groups.
        /// </summary>
        /// <param name="items">Template groups to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Created template groups.</returns>
        public async Task<IEnumerable<TemplateGroup>> CreateAsync(IEnumerable<TemplateGroupCreate> items, CancellationToken token = default)
        {
            var req = TemplateGroups.create(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Upsert a list of template groups.
        /// </summary>
        /// <param name="items">Template groups to upsert.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Upserted template groups.</returns>
        public async Task<IEnumerable<TemplateGroup>> UpsertAsync(IEnumerable<TemplateGroupCreate> items, CancellationToken token = default)
        {
            var req = TemplateGroups.upsert(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of template groups by external id.
        /// </summary>
        /// <param name="items">External ids to retrieve.</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TemplateGroup>> RetrieveAsync(IEnumerable<string> items, bool? ignoreUnknownIds, CancellationToken token = default)
        {
            var req = TemplateGroups.retrieve(items, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List template groups with optional filter.
        /// </summary>
        /// <param name="query">Query with filter, cursor, limit.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of template groups with cursor if number of results exceeds limit.</returns>
        public async Task<ItemsWithCursor<TemplateGroup>> FilterAsync(TemplateGroupQuery query, CancellationToken token = default)
        {
            var req = TemplateGroups.filter(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of template groups by external id.
        /// </summary>
        /// <param name="items">ExternalIds of template groups to delete.</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task DeleteAsync(IEnumerable<string> items, bool ignoreUnknownIds, CancellationToken token = default)
        {
            var req = TemplateGroups.delete(items, ignoreUnknownIds, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Upsert a version. If the version number is left out on <paramref name="item"/>, a new version is created.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="item">Template group version to upsert.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Upserted template version.</returns>
        public async Task<TemplateVersion> UpsertVersionAsync(string externalId, TemplateVersionCreate item, CancellationToken token = default)
        {
            var req = TemplateGroups.upsertVersion(externalId, item, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Filter versions in the given template group, with optional filter. Returns a cursor if the number of results exceed given limit.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="query">Query with cursor, filter, limit.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Retrieved items with optional cursor if number of results exceed limit.</returns>
        public async Task<ItemsWithCursor<TemplateVersion>> FilterVersionsAsync(string externalId, TemplateVersionQuery query, CancellationToken token = default)
        {
            var req = TemplateGroups.filterVersions(externalId, query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a given version from the template group.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Version to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task DeleteVersionsAsync(string externalId, int version, CancellationToken token = default)
        {
            var req = TemplateGroups.deleteVersions(externalId, version, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a list of template instances.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template instances to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Created template instances.</returns>
        public async Task<IEnumerable<TemplateInstance>> CreateInstancesAsync(string externalId, int version, IEnumerable<TemplateInstanceCreate> items, CancellationToken token = default)
        {
            var req = TemplateGroups.createInstances(externalId, version, items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Upsert a list of template instances.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template instances to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Upserted template instances.</returns>
        public async Task<IEnumerable<TemplateInstance>> UpsertInstancesAsync(string externalId, int version, IEnumerable<TemplateInstanceCreate> items, CancellationToken token = default)
        {
            var req = TemplateGroups.upsertInstances(externalId, version, items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a list of template instances.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template instance updates.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Updated template instances.</returns>
        public async Task<IEnumerable<TemplateInstance>> UpdateInstancesAsync(string externalId, int version, IEnumerable<UpdateItem<TemplateInstanceUpdate>> items, CancellationToken token = default)
        {
            var req = TemplateGroups.updateInstances(externalId, version, items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of template instances.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template instance externalIds to retrieve.</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Retrieved template instances.</returns>
        public async Task<IEnumerable<TemplateInstance>> RetrieveInstancesAsync(string externalId, int version, IEnumerable<string> items, bool? ignoreUnknownIds, CancellationToken token = default)
        {
            var req = TemplateGroups.retrieveInstances(externalId, version, items, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of template instances.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template instance externalIds</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task DeleteInstancesAsync(string externalId, int version, IEnumerable<string> items, bool ignoreUnknownIds, CancellationToken token = default)
        {
            var req = TemplateGroups.deleteInstances(externalId, version, items, ignoreUnknownIds, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a list of template views.
        /// </summary>
        /// <typeparam name="TFilter">Generic filter type.</typeparam>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template views to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Created template views.</returns>
        public async Task<IEnumerable<TemplateView<TFilter>>> CreateViewsAsync<TFilter>(string externalId, int version, IEnumerable<TemplateViewCreate<TFilter>> items, CancellationToken token = default)
        {
            var req = TemplateGroups.createViews(externalId, version, items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Upsert a list of template views.
        /// </summary>
        /// <typeparam name="TFilter">Generic filter type.</typeparam>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template views to upsert.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Upserted template views.</returns>
        public async Task<IEnumerable<TemplateView<TFilter>>> UpsertViewsAsync<TFilter>(string externalId, int version, IEnumerable<TemplateViewCreate<TFilter>> items, CancellationToken token = default)
        {
            var req = TemplateGroups.upsertViews(externalId, version, items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List views for the given template version.
        /// </summary>
        /// <typeparam name="TFilter">Type of filter in the query.</typeparam>
        /// <typeparam name="TResultFilter">Type of filter in the result.</typeparam>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="query">Query with optional filter, limit, cursor.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Retrieved template views, with cursor if number of results exceed limit.</returns>
        public async Task<ItemsWithCursor<TemplateView<TResultFilter>>> FilterViewsAsync<TFilter, TResultFilter>(string externalId, int version, TemplateViewFilterQuery<TFilter> query, CancellationToken token = default)
        {
            var req = TemplateGroups.filterViews<TFilter, TResultFilter>(externalId, version, query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Resolve view, listing results with optional cursor.
        /// </summary>
        /// <typeparam name="TFilter">Type of filter in request.</typeparam>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="query">Query for view resolution.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of results with cursor if number of results exceed limit.</returns>
        public async Task<ItemsWithCursor<TResult>> ResolveViewAsync<TFilter, TResult>(string externalId, int version, TemplateViewResolveRequest<TFilter> query, CancellationToken token = default)
        {
            var req = TemplateGroups.resolveView<TFilter, TResult>(externalId, version, query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of views by view external id, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="items">Template view external ids.</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task DeleteViewsAsync(string externalId, int version, IEnumerable<string> items, bool ignoreUnknownIds, CancellationToken token = default)
        {
            var req = TemplateGroups.deleteViews(externalId, version, items, ignoreUnknownIds, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Query the given template group version with graphql.
        /// </summary>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="externalId">Template group externalId.</param>
        /// <param name="version">Template version.</param>
        /// <param name="query">Graphql query description.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Result of type <typeparamref name="TResult"/> with list of errors.</returns>
        public async Task<GraphQlResult<TResult>> QueryAsync<TResult>(string externalId, int version, GraphQlQuery query, CancellationToken token = default)
        {
            var req = TemplateGroups.query<TResult>(externalId, version, query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
