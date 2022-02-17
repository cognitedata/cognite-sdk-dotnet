using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.FSharp.Core;

using Oryx;
using Oryx.Pipeline;


namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all transformations resources
    /// </summary>
    public class TransformationsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal TransformationsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        #region retrieve_configs
        /// <summary>
        /// Retrieve a list of transformations by internal or external id. Optionally ignoring unknown ids.
        /// </summary>
        /// <param name="items">Query for items to retrieve</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The list of retrieved transformations</returns>
        public async Task<IEnumerable<Transformation>> RetrieveAsync(TransformationRetrieve items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.retrieve(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of transformations by internal or external id. Optionally ignoring unknown ids.
        /// </summary>
        /// <param name="items">List of transformations to retrieve.</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids.</param>
        /// <param name="withJobDetails">True to include information about last running and created jobs.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The list of retrieved transformations</returns>
        public async Task<IEnumerable<Transformation>> RetrieveAsync(IEnumerable<Identity> items, bool ignoreUnknownIds = false, bool withJobDetails = false, CancellationToken token = default)
        {
            return await RetrieveAsync(new TransformationRetrieve { Items = items, IgnoreUnknownIds = ignoreUnknownIds, WithJobDetails = withJobDetails }, token).ConfigureAwait(false);
        }

        #endregion

        /// <summary>
        /// List transformations with optional filter and pagination.
        /// </summary>
        /// <param name="query">Query with filter</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of transformations with cursor for pagination</returns>
        public async Task<ItemsWithCursor<Transformation>> FilterAsync(TransformationFilterQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.Transformations.filter(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        #region delete
        /// <summary>
        /// Delete a list of transformations, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="items">Items to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(TransformationDelete items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.delete(items, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of transformations, optionally ignor unknown ids.
        /// </summary>
        /// <param name="items">Ids of items to delete</param>
        /// <param name="ignoreUnknownIds">True to ignore transformations not present in fusion</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(IEnumerable<Identity> items, bool ignoreUnknownIds = false, CancellationToken token = default)
        {
            await DeleteAsync(new TransformationDelete { Items = items, IgnoreUnknownIds = ignoreUnknownIds }, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of transformations, optionally ignor unknown ids.
        /// </summary>
        /// <param name="items">Internal ids of items to delete</param>
        /// <param name="ignoreUnknownIds">True to ignore transformations not present in fusion</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(IEnumerable<long> items, bool ignoreUnknownIds = false, CancellationToken token = default)
        {
            await DeleteAsync(new TransformationDelete { Items = items.Select(Identity.Create), IgnoreUnknownIds = ignoreUnknownIds }, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of transformations, optionally ignor unknown ids.
        /// </summary>
        /// <param name="items">External ids of items to delete</param>
        /// <param name="ignoreUnknownIds">True to ignore transformations not present in fusion</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(IEnumerable<string> items, bool ignoreUnknownIds = false, CancellationToken token = default)
        {
            await DeleteAsync(new TransformationDelete { Items = items.Select(Identity.Create), IgnoreUnknownIds = ignoreUnknownIds }, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// List transformation with pagination.
        /// </summary>
        /// <param name="query">Query to use</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of transformations with cursor for pagination</returns>
        public async Task<ItemsWithCursor<Transformation>> ListAsync(TransformationQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.Transformations.list(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a list of transformations.
        /// </summary>
        /// <param name="items">Transformations to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of created transformations.</returns>
        public async Task<IEnumerable<Transformation>> CreateAsync(IEnumerable<TransformationCreate> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.create(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a list of transformations by internal or external id.
        /// </summary>
        /// <param name="items">Transformation updates</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Updated transformations</returns>
        public async Task<IEnumerable<Transformation>> UpdateAsync(IEnumerable<UpdateItem<TransformationUpdate>> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.update(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Run a transformation, creating a job scheduled to run immediately.
        /// </summary>
        /// <param name="item">Id of transformation to run</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Scheduled transformation job</returns>
        public async Task<TransformationJob> RunAsync(Identity item, CancellationToken token = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            var req = Oryx.Cognite.Transformations.run(item, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List jobs optionally filtered to a single transformation, with pagination.
        /// </summary>
        /// <param name="query">Job query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of jobs with cursor for pagination</returns>
        public async Task<ItemsWithCursor<TransformationJob>> ListJobsAsync(TransformationJobQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.Transformations.listJobs(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve jobs by job internal id, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="items">Internal id of jobs to retrieve.</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids, default false.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Retrieved jobs</returns>
        public async Task<IEnumerable<TransformationJob>> RetrieveJobsAsync(IEnumerable<long> items, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.retrieveJobs(items, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List metrics for the given job, such as number of rows read or items created.
        /// </summary>
        /// <param name="id">Job internal id</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of retrieved job metrics.</returns>
        public async Task<IEnumerable<TransformationJobMetric>> ListJobMetricsAsync(long id, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Transformations.listJobMetrics(id, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of transformation schedules by transformation id.
        /// </summary>
        /// <param name="items">Ids of the transformations to retrieve schedules for</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids, default false</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of retrieved schedules</returns>
        public async Task<IEnumerable<TransformationSchedule>> RetrieveSchedulesAsync(IEnumerable<Identity> items, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.retrieveSchedules(items, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List schedules for all transformations, with pagination.
        /// </summary>
        /// <param name="query">Query for listing schedules</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of schedules with cursor for pagination</returns>
        public async Task<ItemsWithCursor<TransformationSchedule>> ListSchedulesAsync(TransformationScheduleQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.Transformations.listSchedules(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Create schedules for a list of transformations.
        /// </summary>
        /// <param name="items">Schedules to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of created schedules</returns>
        public async Task<IEnumerable<TransformationSchedule>> ScheduleAsync(IEnumerable<TransformationScheduleCreate> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.schedule(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
        #region unschedule
        /// <summary>
        /// Delete schedules for a list of transformations, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="items">Transformation schedules to delete.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task UnscheduleAsync(TransformationScheduleDelete items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.unschedule(items, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete schedules for a list of transformations, optionally ignoring unknown ids.
        /// </summary>
        /// <param name="items">Ids of transformations to delete schedules for.</param>
        /// <param name="ignoreUnknownIds">True to ignore unknown ids, default false</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task UnscheduleAsync(IEnumerable<Identity> items, bool ignoreUnknownIds = false, CancellationToken token = default)
        {
            await UnscheduleAsync(new TransformationScheduleDelete { Items = items, IgnoreUnknownIds = ignoreUnknownIds }, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Update schedules for a list of transformations.
        /// </summary>
        /// <param name="items">Transformation schdules to update</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of updated schedules</returns>
        public async Task<IEnumerable<TransformationSchedule>> UpdateSchedulesAsync(IEnumerable<UpdateItem<TransformationScheduleUpdate>> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.updateSchedules(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List notifications, optionally restricting it to a single transformation, with pagination.
        /// </summary>
        /// <param name="query">Query with some optional filters</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of notifications</returns>
        public async Task<ItemsWithCursor<TransformationNotification>> ListNotificationsAsync(TransformationNotificationQuery query, CancellationToken token = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var req = Oryx.Cognite.Transformations.listNotifications(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to notifications for multiple transformations.
        /// </summary>
        /// <param name="items">Transformation notification subscriptions to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Created notification subscriptions</returns>
        public async Task<IEnumerable<TransformationNotification>> SubscribeAsync(IEnumerable<TransformationNotificationCreate> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.subscribe(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete notification subscriptions by notification internal id.
        /// </summary>
        /// <param name="items">Notification subscriptions to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task UnsubscribeAsync(IEnumerable<long> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var req = Oryx.Cognite.Transformations.deleteNotifications(items, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Run a transformation query to preview the result.
        /// </summary>
        /// <param name="preview">Query to run</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Preview result and schema</returns>
        public async Task<TransformationPreviewResult> PreviewAsync(TransformationPreview preview, CancellationToken token = default)
        {
            if (preview is null) throw new ArgumentNullException(nameof(preview));

            var req = Oryx.Cognite.Transformations.preview(preview, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the schema for a given destination type and conflict mode.
        /// </summary>
        /// <param name="schemaType">Transformation destination type</param>
        /// <param name="conflictMode">Transformation conflict mode</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of column type definitions making up the schema</returns>
        public async Task<IEnumerable<TransformationColumnType>> GetSchemaAsync(TransformationDestinationType schemaType, TransformationConflictMode conflictMode, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Transformations.getSchema(schemaType, conflictMode, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
