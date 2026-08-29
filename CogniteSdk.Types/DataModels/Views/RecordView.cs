// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// A view used to ingest or query records.
    /// </summary>
    public class RecordView : View
    {
        /// <summary>
        /// External IDs of the stream this view targets.
        /// </summary>
        public IEnumerable<string> StreamId { get; set; }

        /// <summary>
        /// Constructor initializing UsedFor to record.
        /// </summary>
        public RecordView()
        {
            UsedFor = UsedFor.record;
        }
    }
}
