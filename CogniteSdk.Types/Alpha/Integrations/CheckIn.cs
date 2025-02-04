// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Type of task update.
    /// </summary>
    public enum TaskUpdateType
    {
        /// <summary>
        /// Task was started.
        /// </summary>
        started,
        /// <summary>
        /// Task ended.
        /// </summary>
        ended
    }

    /// <summary>
    /// Update to the status of a task.
    /// </summary>
    public class TaskUpdate
    {
        /// <summary>
        /// Type of task update.
        /// </summary>
        public TaskUpdateType Type { get; set; }
        /// <summary>
        /// Name of the task to update, must correspond to a task in the task array given
        /// during calls to `extractorinfo`.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The time this task update occured on the extractor side.
        /// Note that task updates are checked to ensure that they are correct when applied sequentially,
        /// i.e. that tasks are started after they end etc.
        /// </summary>
        public long Timestamp { get; set; }
    }

    /// <summary>
    /// Error severity level.
    /// </summary>
    public enum ErrorLevel
    {
        /// <summary>
        /// Something wrong happened but the extractor was able to continue.
        /// </summary>
        warning,
        /// <summary>
        /// Something wrong happened and the extractor _may_ be able to recover,
        /// but there will likely be downtime and/or loss of data.
        /// </summary>
        error,
        /// <summary>
        /// Something wrong happened that cannot be recovered from.
        /// After reporting a fatal error the task should end.
        /// </summary>
        fatal,
    }

    /// <summary>
    /// Error with optional associated task.
    /// </summary>
    public class ErrorWithTask
    {
        /// <summary>
        /// Error external ID. Errors may be updated by reporting them again with the
        /// same external ID.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Error level.
        /// </summary>
        public ErrorLevel Level { get; set; }
        /// <summary>
        /// Short error description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Long error details, may contain details such as stack traces.
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// Name of the task this error is associated with. This task must be running when the error occurs.
        /// If the task is left out or null, the error is associated with the extractor in general.
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Time the error started.
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// Time the error ended. Set this equal to `StartTime` if the error is instantaneous.
        /// </summary>
        public long? EndTime { get; set; }
    }

    /// <summary>
    /// Request sent to integrations to report liveness of the extractor and receive any notifications
    /// it needs to consider.
    /// 
    /// All fields except `ExternalId` are optional, and an empty checkin still has the semantic
    /// meaning of reporting that the extractor is alive.
    /// </summary>
    public class CheckInRequest
    {
        /// <summary>
        /// ExternalId of the integration to report checkin to.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Started and ended tasks.
        /// </summary>
        public IEnumerable<TaskUpdate> TaskEvents { get; set; }
        /// <summary>
        /// Errors and warnings.
        /// </summary>
        public IEnumerable<ErrorWithTask> Errors { get; set; }
    }

    /// <summary>
    /// Response to a checkin request.
    /// </summary>
    public class CheckInResponse
    {
        /// <summary>
        /// Integration external ID.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Last stored extractor configuration revision. The extractor may use this
        /// to decide to restart with a new remote configuration file.
        /// </summary>
        public int? LastConfigRevision { get; set; }
    }

    /// <summary>
    /// Report changes to general information about the extractor.
    /// This is typically used on extractor startup and when loading a new config file.
    /// </summary>
    public class StartupRequest
    {
        /// <summary>
        /// ID of the running extractor.
        /// </summary>
        public ExtractorId Extractor { get; set; }
        /// <summary>
        /// List of tasks configured for this extractor.
        /// </summary>
        public IEnumerable<IntegrationTask> Tasks { get; set; }
        /// <summary>
        /// Active config revision, either the revision number or "local" if a local config file is
        /// currently in use.
        /// </summary>
        public StringOrInt ActiveConfigRevision { get; set; }
    }
}