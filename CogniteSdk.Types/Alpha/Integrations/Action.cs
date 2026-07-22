// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Status of a triggered action.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActionStatus
    {
        /// <summary>Action has been created and is waiting to be picked up by the extractor.</summary>
        pending,
        /// <summary>Action is currently being executed by the extractor.</summary>
        running,
        /// <summary>Action execution failed.</summary>
        failed,
        /// <summary>Action execution completed successfully.</summary>
        succeeded,
        /// <summary>Cancellation has been requested but not yet confirmed by the extractor.</summary>
        cancel_pending,
        /// <summary>Action was canceled.</summary>
        canceled,
    }

    /// <summary>
    /// Type of an available action.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActionType
    {
        /// <summary>Action starts a task on the extractor.</summary>
        start_task,
        /// <summary>Action stops a running task on the extractor.</summary>
        stop_task,
        /// <summary>Custom action with semantics defined by the extractor.</summary>
        custom,
    }

    /// <summary>
    /// Declares an available action that the extractor can handle.
    /// Reported to the integrations API during startup.
    /// </summary>
    public class AvailableActionWrite
    {
        /// <summary>
        /// Name of the action, unique per integration.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type of the action.
        /// </summary>
        public ActionType Type { get; set; }
        /// <summary>
        /// Human-readable description of what the action does.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// For <see cref="ActionType.start_task"/> and <see cref="ActionType.stop_task"/>,
        /// the name of the associated task. Must match a task name reported during startup.
        /// </summary>
        public string Task { get; set; }
    }

    /// <summary>
    /// Update sent by the extractor during check-in to report progress or completion of an action.
    /// </summary>
    public class ActionUpdate
    {
        /// <summary>
        /// External ID of the action to update.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// New status of the action. Extractors may only set:
        /// <see cref="ActionStatus.running"/>, <see cref="ActionStatus.failed"/>,
        /// <see cref="ActionStatus.succeeded"/>, or <see cref="ActionStatus.canceled"/>.
        /// </summary>
        public ActionStatus? Status { get; set; }
        /// <summary>
        /// Human-readable message describing the result or reason for the status.
        /// </summary>
        public string ResultMessage { get; set; }
        /// <summary>
        /// Structured key-value metadata about the result.
        /// </summary>
        public IDictionary<string, string> ResultMetadata { get; set; }
    }

    /// <summary>
    /// A triggered action returned from the integrations API.
    /// Named <c>IntegrationAction</c> to avoid collision with <see cref="System.Action"/>.
    /// </summary>
    public class IntegrationAction
    {
        /// <summary>
        /// Action external ID.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Name of the available action this was triggered against.
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// Current status of the action.
        /// </summary>
        public ActionStatus Status { get; set; }
        /// <summary>
        /// Optional key-value metadata provided by the caller when the action was triggered.
        /// </summary>
        public IDictionary<string, string> CallMetadata { get; set; }
        /// <summary>
        /// Time this action was created, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Time this action was last updated, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// Optional message describing the result of the action.
        /// </summary>
        public string ResultMessage { get; set; }
        /// <summary>
        /// Optional structured metadata about the result set by the extractor.
        /// </summary>
        public IDictionary<string, string> ResultMetadata { get; set; }
    }

    /// <summary>
    /// Request item for creating a new triggered action.
    /// </summary>
    public class CreateAction
    {
        /// <summary>
        /// External ID for the action, must be unique per project.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Name of the available action to trigger.
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// Optional key-value metadata to pass to the extractor when the action is executed.
        /// </summary>
        public IDictionary<string, string> CallMetadata { get; set; }
    }

    /// <summary>
    /// Request to retrieve a list of actions by external ID.
    /// </summary>
    public class ActionsRetrieve : ItemsWithIgnoreUnknownIds<CogniteExternalId> { }

    /// <summary>
    /// Request to cancel a list of actions by external ID.
    /// </summary>
    public class CancelActionsRequest : ItemsWithIgnoreUnknownIds<CogniteExternalId> { }

    /// <summary>
    /// Query for listing actions.
    /// </summary>
    public class ActionsQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter actions belonging to a specific integration.
        /// </summary>
        public string Integration { get; set; }
        /// <summary>
        /// Only return actions created after this time (milliseconds since epoch).
        /// </summary>
        public long? CreatedAfter { get; set; }
        /// <summary>
        /// Whether to include completed actions (succeeded, failed, canceled).
        /// Defaults to <c>true</c> in the API when not specified.
        /// </summary>
        public bool? IncludeCompleted { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var qs = base.ToQueryParams();
            if (Integration != null) qs.Add(("integration", Integration));
            if (CreatedAfter.HasValue) qs.Add(("createdAfter", CreatedAfter.Value.ToString()));
            if (IncludeCompleted.HasValue) qs.Add(("includeCompleted", IncludeCompleted.Value ? "true" : "false"));
            return qs;
        }
    }

    /// <summary>
    /// Query that carries the integration external ID as a query parameter.
    /// Used when creating actions, where the integration external ID is a required query param.
    /// </summary>
    public class ActionsCreateQuery : IQueryParams
    {
        private readonly string _integrationExternalId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="integrationExternalId">Integration external ID to include as a query parameter.</param>
        public ActionsCreateQuery(string integrationExternalId)
        {
            _integrationExternalId = integrationExternalId ?? throw new System.ArgumentNullException(nameof(integrationExternalId));
        }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams() =>
            new List<(string, string)> { ("externalId", _integrationExternalId) };
    }
}
