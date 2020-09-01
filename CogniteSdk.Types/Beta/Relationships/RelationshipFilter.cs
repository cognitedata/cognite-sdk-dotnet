// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for filtering relationships.
    /// </summary>
    public class RelationshipFilter
    {
        /// <summary>
        /// Sources to include.
        /// </summary>
        public IEnumerable<string> SourceExternalIds { get; set; }

        /// <summary>
        /// Include relationships which source is of any of the specified resource types
        /// Valid values are: "asset", "timeseries", "file", "event", "sequence"
        /// </summary>
        public IEnumerable<string> SourceTypes {get; set;}

        /// <summary>
        /// Targets to include.
        /// </summary>
        public IEnumerable<string> TargetExternalIds { get; set; }

        /// <summary>
        /// Include relationships which target is of any of the specified resource types
        /// Valid values are: "asset", "timeseries", "file", "event", "sequence"
        /// </summary>
        public IEnumerable<string> TargetTypes { get; set; }

        /// <summary>
        /// Include relationships that has any of the provided values in their DataSetId field.
        /// </summary>
        public IEnumerable<Int64> DataSetIds { get; set; }

        /// <summary>
        /// Range for startTime.
        /// </summary>
        public TimeRange StartTime { get; set; }

        /// <summary>
        /// Range for EndTime.
        /// </summary>
        public TimeRange EndTime { get; set; }

        /// <summary>
        /// Range for Confidence.
        /// </summary>
        public RangeObject Confidence { get; set; }

        /// <summary>
        /// Range for CreatedTime.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Range for LastUpdatedTime.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }

        /// <summary>
        /// Limits results to those active at this time, i.e. ActiveAtTime falls between StartTime and EndTime. StartTime is treated as inclusive (if activeAtTime ie equal to StartTime then the relationship will be included). EndTime is treated as exclusive (if ActiveTime is equal to EndTime then the relationship will NOT be included). If a relationship has neither StartTime nor EndTime, the relationship is active at all times.
        /// </summary>
        public TimeRange ActiveAtTime { get; set; }

        /// <summary>
        /// Limits result to Relationships that match the LabelFilter.
        /// </summary>
        public LabelFilter Labels { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
