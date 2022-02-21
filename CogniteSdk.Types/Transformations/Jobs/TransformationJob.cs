// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Status of a transformation job.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransformationJobStatus
    {
        /// <summary>
        /// Job is running
        /// </summary>
        Running = 0,
        /// <summary>
        /// Job has been created, but is not yet running.
        /// </summary>
        Created = 1,
        /// <summary>
        /// Job has been completed successfully.
        /// </summary>
        Completed = 2,
        /// <summary>
        /// Job has failed.
        /// </summary>
        Failed = 3
    }

    /// <summary>
    /// Description of transformation job read from CDF.
    /// </summary>
    public class TransformationJob
    {
        /// <summary>
        /// Id of transformation job.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id of transformation this job belongs to.
        /// </summary>
        public long TransformationId { get; set; }

        /// <summary>
        /// External id of transformation this job belongs to.
        /// </summary>
        public string TransformationExternalId { get; set; }

        /// <summary>
        /// Project this job reads from.
        /// </summary>
        public string SourceProject { get; set; }

        /// <summary>
        /// Project this job writes to.
        /// </summary>
        public string DestinationProject { get; set; }

        /// <summary>
        /// Destination of transformation this job belongs to.
        /// </summary>
        public TransformationDestination Destination { get; set; }

        /// <summary>
        /// How this job handles conflicts.
        /// </summary>
        public TransformationConflictMode ConflictMode { get; set; }

        /// <summary>
        /// Query this job is executing.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// How null fields are handled by this job.
        /// </summary>
        public bool IgnoreNullFields { get; set; }

        /// <summary>
        /// Status of job.
        /// </summary>
        public TransformationJobStatus Status { get; set; }

        /// <summary>
        /// If the job failed, this will be a string describing why.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// When the job was created:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds
        /// </summary>
        public long? CreatedTime { get; set; }

        /// <summary>
        /// When the job was started:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long? StartedTime { get; set; }

        /// <summary>
        /// When the job finished.
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long? FinishedTime { get; set; }

        /// <summary>
        /// When the job was last registered as running:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long? LastSeenTime { get; set; }
    }
}
