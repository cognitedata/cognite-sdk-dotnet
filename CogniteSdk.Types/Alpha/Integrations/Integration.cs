// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// ID of an extractor.
    /// </summary>
    public class ExtractorId
    {
        /// <summary>
        /// Extractor external ID. If this starts with `cognite` it must
        /// correspond to a cognite extractor.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Extractor version. For cognite extractors this must match a published extractor
        /// version.
        /// </summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// Task type.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskType
    {
        /// <summary>
        /// Task runs continuously.
        /// </summary>
        continuous,
        /// <summary>
        /// Task runs for finite periods of time.
        /// </summary>
        batch,
    }

    /// <summary>
    /// Integration task.
    /// </summary>
    public class IntegrationTask
    {
        /// <summary>
        /// Task type.
        /// </summary>
        public TaskType Type { get; set; }
        /// <summary>
        /// Task name, must be unique per integration.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Task description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Whether or not this task can be triggered through an external action.
        /// </summary>
        public bool Action { get; set; }
    }

    /// <summary>
    /// Integration, representing an on-premise program that is managed through CDF.
    /// </summary>
    public class Integration
    {
        /// <summary>
        /// Integration external ID, must be unique per project.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Integration name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Integration description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ID of the running extractor.
        /// </summary>
        public ExtractorId Extractor { get; set; }
        /// <summary>
        /// Time in milliseconds since Jan 1. 1970 this extractor last checked in.
        /// </summary>
        public long? LastSeen { get; set; }
        /// <summary>
        /// Last published configuration revision.
        /// </summary>
        public int? LastConfigRevision { get; set; }
        /// <summary>
        /// Active config revision, either the revision number or "local" if a local config file is
        /// currently in use.
        /// </summary>
        public StringOrInt ActiveConfigRevision { get; set; }
        /// <summary>
        /// List of tasks on this integration.
        /// </summary>
        public IEnumerable<IntegrationTask> Tasks { get; set; }
        /// <summary>
        /// Time when this integration was created in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Time when this integration was last updated in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }

    /// <summary>
    /// Create a new integration.
    /// </summary>
    public class CreateIntegration
    {
        /// <summary>
        /// Integration external ID, must be unique per project.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Integration name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Integration description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ID of the running extractor.
        /// Note that this may be changed by the extractor itself once it starts running.
        /// </summary>
        public ExtractorId Extractor { get; set; }
    }

    /// <summary>
    /// Update an integration.
    /// </summary>
    public class UpdateIntegration
    {
        /// <summary>
        /// Human readable integration name.
        /// </summary>
        public UpdateNullable<string> Name { get; set; }
        /// <summary>
        /// Integration description.
        /// </summary>
        public UpdateNullable<string> Description { get; set; }
    }

    /// <summary>
    /// Request to delete a list of integrations.
    /// </summary>
    public class IntegrationsDelete : ItemsWithIgnoreUnknownIds<CogniteExternalId> { }

    /// <summary>
    /// Request to retrieve a list of integrations.
    /// </summary>
    public class IntegrationsRetrieve : ItemsWithIgnoreUnknownIds<CogniteExternalId> { }

    /// <summary>
    /// Query for retrieving integration errors.
    /// </summary>
    public class ErrorsQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter by task histories belonging to a specific integration.
        /// </summary>
        public string Integration { get; set; }
        /// <summary>
        /// Return task histories belonging to a specific task. Requires integration to be set.
        /// </summary>
        public string TaskName { get; set; }
        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var qs = base.ToQueryParams();
            if (Integration != null) qs.Add(("integration", Integration));
            if (TaskName != null) qs.Add(("taskName", TaskName));
            return qs;
        }
    }

    /// <summary>
    /// Query for retrieving integrations.
    /// </summary>
    public class IntegrationsQuery : CursorQueryBase { }
}