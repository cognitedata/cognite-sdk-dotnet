// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Events
{
    public class EventQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on assets with strict matching.
        /// </summary>
        public EventFilterDto Filter { get; set; }

        /// <summary>
        /// Sort by array of selected fields. Syntax: ["<fieldname>:asc|desc"]. Default sort order is asc with short
        /// syntax: ["<fieldname>"]. Filter accepts the following field names: startTime, endTime, createdTime,
        /// lastUpdatedTime. Partitions are done independently of sorting, there is no guarantee on sort order between
        /// elements from different partitions.
        /// </summary>
        public IEnumerable<string> Sort { get; set; }

        /// <summary>
        /// Splits the data set into N partitions. You need to follow the cursors within each partition in order to
        /// receive all the data. Example: 1/10.
        /// </summary>
        public string Partition { get; set; }
    }
}