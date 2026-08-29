// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Create or update a view used to ingest or query records.
    /// </summary>
    public class RecordViewCreate : ViewCreate
    {
        /// <summary>
        /// External IDs of the stream this view targets.
        /// Record views only support mapped properties that reference record containers.
        /// Connections are not supported.
        /// </summary>
        public IEnumerable<string> StreamId { get; set; }
    }
}
