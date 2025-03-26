// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevisionLog read class.
    /// </summary>
    public class ThreeDRevisionLog
    {
        /// <summary>
        /// The creation time of the resource, in milliseconds since January 1, 1970 at 00:00 UTC.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// How severe is the message (3 = INFO, 5 = WARN, 7 = ERROR).
        /// </summary>
        public int Severity { get; set; }

        /// <summary>
        /// Main computer parsable log entry type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Optional extra information related to the log entry.
        /// </summary>
        public string Info { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

