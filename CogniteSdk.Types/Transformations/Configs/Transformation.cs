// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Transformation behavior when data already exists.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransformationConflictMode
    {
        /// <summary>
        /// Update or insert.
        /// </summary>
        upsert,
        /// <summary>
        /// Abort if a conflict is encountered.
        /// </summary>
        abort,
        /// <summary>
        /// Update existing data, fail if it does not exist.
        /// </summary>
        update,
        /// <summary>
        /// Delete any matches.
        /// </summary>
        delete
    }

    /// <summary>
    /// Represents the configuration of a transformation read from CDF.
    /// </summary>
    public class Transformation
    {
        /// <summary>
        /// Internal id of transformation.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// External id of transformation.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Name of transformation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Transformation SQL query.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Object describing what the result of the transformation is written to.
        /// </summary>
        public TransformationDestination Destination { get; set; }

        /// <summary>
        /// How to handle when data already exists in the destination.
        /// </summary>
        public TransformationConflictMode ConflictMode { get; set; }

        /// <summary>
        /// How NULL values are ignored on update. If true, they are ignored, otherwise the destination
        /// field is set to null.
        /// </summary>
        public bool IgnoreNullFields { get; set; }

        /// <summary>
        /// True if transformation is visible to all in project, false if only to owner.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// When the transformation was created:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// When the transformation was last updated:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <summary>
        /// Owner of the transformation.
        /// </summary>
        public TransformationOwner Owner { get; set; }

        /// <summary>
        /// Indicates if the transformation belongs to the current user.
        /// </summary>
        public bool OwnerIsCurrentUser { get; set; }

        /// <summary>
        /// Indicates if the transformation is configured with a source api key.
        /// </summary>
        public bool HasSourceApiKey { get; set; }

        /// <summary>
        /// Indicates if the transformation is configured with a destination api key.
        /// </summary>
        public bool HasDestinationApiKey { get; set; }

        /// <summary>
        /// Indicates if the transformation is configured with source oidc credentials.
        /// </summary>
        public bool HasSourceOidcCredentials { get; set; }

        /// <summary>
        /// Indicates if the transformation is configured with destination oidc credentials.
        /// </summary>
        public bool HasDestinationOidcCredentials { get; set; }

        /// <summary>
        /// Details for the last finished job of this transformation.
        /// </summary>
        public TransformationJob LastFinishedJob { get; set; }

        /// <summary>
        /// Details for the running job for this transformation, if one exists.
        /// </summary>
        public TransformationJob RunningJob { get; set; }

        /// <summary>
        /// Details for the schedule if the transformation is scheduled.
        /// </summary>
        public TransformationSchedule Schedule { get; set; }

        /// <summary>
        /// Gives information about whether the transformation is blocked.
        /// </summary>
        public TransformationBlockedInfo Blocked { get; set; }
    }
}
