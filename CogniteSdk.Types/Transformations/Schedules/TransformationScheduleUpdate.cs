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

    /// <summary>
    /// Item containing an update to a transformation schedule by transformation internal or external id.
    /// </summary>
    public class TransformationScheduleUpdateItem : UpdateItem<TransformationScheduleUpdate>
    {
        /// <summary>
        /// Initialize transformation schedule update item with external id.
        /// </summary>
        /// <param name="externalId">Transformation external id</param>
        public TransformationScheduleUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize transformation schedule update item with internal id.
        /// </summary>
        /// <param name="internalId">Transformation internal id</param>
        public TransformationScheduleUpdateItem(long internalId) : base(internalId)
        {
        }
    }
}
