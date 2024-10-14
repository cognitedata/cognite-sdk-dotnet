// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.DataModels.Core
{
    /// <summary>
    /// Core data model representation of an activity.
    /// </summary>
    public class CogniteActivity : CogniteCoreInstanceBase, ICogniteSchedulable
    {
        /// <inheritdoc />
        public DateTime? StartTime { get; set; }
        /// <inheritdoc />
        public DateTime? EndTime { get; set; }
        /// <inheritdoc />
        public DateTime? ScheduledStartTime { get; set; }
        /// <inheritdoc />
        public DateTime? ScheduledEndTime { get; set; }

        /// <summary>
        /// List of assets this activity relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Assets { get; set; }
        /// <summary>
        /// List of equipment this activity relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Equipment { get; set; }
        /// <summary>
        /// List of timeseries this activity relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Timeseries { get; set; }
    }
}