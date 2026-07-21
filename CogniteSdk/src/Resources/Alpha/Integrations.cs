using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Alpha;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite.Alpha;

namespace CogniteSdk.Resources.Alpha
{
    /// <summary>
    /// Resource for the integrations API.
    /// </summary>
    public class IntegrationsResource : Resource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IntegrationsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Check in with the integrations API, reporting any changes that occured since the last checkin
        /// and generally reporting liveness.
        /// </summary>
        /// <param name="request">Checkin request</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Response with changes and notifications the extractor should be aware of.</returns>
        public async Task<CheckInResponse> CheckInAsync(CheckInRequest request, CancellationToken token = default)
        {
            var req = Integrations.checkin(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Report information about the running extractor.
        /// 
        /// This should generally only be called on extractor startup and on changes to config.
        /// </summary>
        /// <param name="request">Changes to extractor info.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Response with changes and notifications the extractor should be aware of.</returns>
        public async Task<CheckInResponse> StartupAsync(StartupRequest request, CancellationToken token = default)
        {
            var req = Integrations.startup(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a list of integrations.
        /// </summary>
        /// <param name="items">Integrations to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created integrations</returns>
        public async Task<IEnumerable<Integration>> CreateAsync(IEnumerable<CreateIntegration> items, CancellationToken token = default)
        {
            var req = Integrations.create(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete integrations with all their tasks, configs and task history.
        /// </summary>
        /// <param name="ids">Integration IDs to delete</param>
        /// <param name="ignoreUnknownIds">Whether to ignore unknown IDs</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task DeleteAsync(IEnumerable<string> ids, bool ignoreUnknownIds, CancellationToken token = default)
        {
            var req = Integrations.delete(new IntegrationsDelete
            {
                Items = ids.Select((id) => new CogniteExternalId(id)).ToList(),
                IgnoreUnknownIds = ignoreUnknownIds
            }, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve integrations, optionally ignoring unknown IDs.
        /// </summary>
        /// <param name="ids">Integration IDs to retrieve.</param>
        /// <param name="ignoreUnknownIds">Whether to ignore unknown IDs</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved integrations</returns>
        public async Task<IEnumerable<Integration>> RetrieveAsync(IEnumerable<string> ids, bool ignoreUnknownIds, CancellationToken token = default)
        {
            var req = Integrations.retrieve(new IntegrationsRetrieve
            {
                Items = ids.Select((id) => new CogniteExternalId(id)).ToList(),
                IgnoreUnknownIds = ignoreUnknownIds
            }, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a list of integrations.
        /// </summary>
        /// <param name="items">Integration updates.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Updated integrations.</returns>
        public async Task<IEnumerable<Integration>> UpdateAsync(IEnumerable<UpdateItem<UpdateIntegration>> items, CancellationToken token = default)
        {
            var req = Integrations.update(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a configuration revision for the given extractor.
        /// </summary>
        /// <param name="revision">Configuration revision to update.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created config revision</returns>
        public async Task<ConfigRevision> CreateConfigRevisionAsync(CreateConfigRevision revision, CancellationToken token = default)
        {
            var req = Integrations.createConfigRevision(revision, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a specific configuration revision, or the latest if <paramref name="revision"/> is left out.
        /// </summary>
        /// <param name="integration">Integration to get the config for.</param>
        /// <param name="revision">Config revision to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Config revision</returns>
        public async Task<ConfigRevision> GetConfigRevisionAsync(string integration, int? revision = null, CancellationToken token = default)
        {
            var req = Integrations.getConfigRevision(integration, revision, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List all config revisions, optionally filtering by integration.
        /// </summary>
        /// <param name="integration">Integration to fetch config revisions for.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of config revisions.</returns>
        public async Task<IEnumerable<ConfigRevisionMetadata>> ListConfigRevisionsAsync(string integration, CancellationToken token = default)
        {
            var req = Integrations.listConfigRevisions(integration, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List task history entries for a given integration.
        /// </summary>
        /// <param name="query">Query for fetching task history.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of task history entries with optional cursor for pagination.</returns>
        public async Task<ItemsWithCursor<TaskHistory>> GetTaskHistoryAsync(TaskHistoryQuery query, CancellationToken token = default)
        {
            var req = Integrations.getTaskHistory(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List errors for a given integration.
        /// </summary>
        /// <param name="query">Query for fetching errors.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of errors with optional cursor for pagination.</returns>
        public async Task<ItemsWithCursor<ErrorWithTask>> ListErrorsAsync(ErrorsQuery query, CancellationToken token = default)
        {
            var req = Integrations.listErrors(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create new triggered actions for the given integration.
        /// </summary>
        /// <param name="integration">External ID of the integration to create actions for.</param>
        /// <param name="items">Actions to create.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created actions.</returns>
        public async Task<IEnumerable<IntegrationAction>> CreateActionsAsync(string integration, IEnumerable<CreateAction> items, CancellationToken token = default)
        {
            if (integration is null) throw new ArgumentNullException(nameof(integration));
            if (items is null) throw new ArgumentNullException(nameof(items));
            var req = Integrations.createActions(integration, items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve actions by external IDs.
        /// </summary>
        /// <param name="ids">External IDs of the actions to retrieve.</param>
        /// <param name="ignoreUnknownIds">Whether to ignore unknown IDs.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved actions.</returns>
        public async Task<IEnumerable<IntegrationAction>> RetrieveActionsAsync(IEnumerable<string> ids, bool ignoreUnknownIds, CancellationToken token = default)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));
            var req = Integrations.retrieveActions(new ActionsRetrieve
            {
                Items = ids.Select(id => new CogniteExternalId(id)).ToList(),
                IgnoreUnknownIds = ignoreUnknownIds
            }, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List actions with optional filtering.
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of actions with optional cursor for pagination.</returns>
        public async Task<ItemsWithCursor<IntegrationAction>> ListActionsAsync(ActionsQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            var req = Integrations.listActions(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel a list of actions by external ID.
        /// </summary>
        /// <param name="ids">External IDs of the actions to cancel.</param>
        /// <param name="ignoreUnknownIds">Whether to ignore unknown IDs.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Cancelled actions.</returns>
        public async Task<IEnumerable<IntegrationAction>> CancelActionsAsync(IEnumerable<string> ids, bool ignoreUnknownIds, CancellationToken token = default)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));
            var req = Integrations.cancelActions(new CancelActionsRequest
            {
                Items = ids.Select(id => new CogniteExternalId(id)).ToList(),
                IgnoreUnknownIds = ignoreUnknownIds
            }, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
