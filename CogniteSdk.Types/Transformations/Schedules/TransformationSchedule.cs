// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// The schedule of a transformation.
    /// </summary>
    public class TransformationSchedule
    {
        /// <summary>
        /// Transformation internal id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Transformation external id.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// When the schedule was created:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// When the schedule was last updated:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <summary>
        /// Cron expression describing when the transformation should be run.
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// If true, transformation is not scheduled.
        /// </summary>
        public bool IsPaused { get; set; }
    }
}
