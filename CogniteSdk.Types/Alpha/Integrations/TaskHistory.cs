// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A historical task run.
    /// </summary>
    public class TaskHistory
    {
        /// <summary>
        /// Name of the task, unique per integration.
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// Number of errors that occured during this task run.
        /// </summary>
        public int ErrorCount { get; set; }
        /// <summary>
        /// When this task run started, in milliseconds since Jan 1. 1970.
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// When this task run ended, in milliseconds since Jan 1. 1970.
        /// If null the task is still running or the extractor is unresponsive.
        /// </summary>
        public long? EndTime { get; set; }
    }

    /// <summary>
    /// Query for retrieving integration task histories.
    /// </summary>
    public class TaskHistoryQuery : CursorQueryBase
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
}
