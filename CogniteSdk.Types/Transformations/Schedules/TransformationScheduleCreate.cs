// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Schedule a transformation by internal or external id.
    /// </summary>
    public class TransformationScheduleCreate : Identity
    {
        /// <summary>
        /// Schedule a transformation by transformation external id.
        /// </summary>
        /// <param name="externalId">Transformation external id</param>
        public TransformationScheduleCreate(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Schedule a transformation by transformation internal id.
        /// </summary>
        /// <param name="internalId">Transformation internal id</param>
        public TransformationScheduleCreate(long internalId) : base(internalId)
        {
        }

        /// <summary>
        /// If true, the transformation is not scheduled.
        /// </summary>
        public bool? IsPaused { get; set; }

        /// <summary>
        /// Cron expression describing when the transformation should be run.
        /// </summary>
        public string Interval { get; set; }
    }
}
