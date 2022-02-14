// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Update the schedule of a transformation.
    /// </summary>
    public class TransformationScheduleUpdate
    {
        /// <summary>
        /// Cron expression describing when the transformation should be run.
        /// </summary>
        public Update<string> Interval { get; set; }

        /// <summary>
        /// If true, the transformation is not scheduled.
        /// </summary>
        public Update<bool> IsPaused { get; set; }
    }
}
