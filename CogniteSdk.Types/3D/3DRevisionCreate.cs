// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevisionCreate read class.
    /// </summary>
    public class ThreeDRevisionCreate
    {
        /// <summary>
        /// True if revision is marked as published. 
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// List of 3 numbers.
        /// </summary>
        public IEnumerable<double> Rotation { get; set; }
        /// <summary>
        /// Initial camera position and target.
        /// </summary>
        public RevisionCameraProperties Camera { get; set; }
        /// <summary>
        ///  Enum: "Queued" "Processing" "Done" "Failed"
        /// The status of the revision.
        /// </summary>

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }
        /// <summary>
        /// The file id.
        /// </summary>
        public long FileId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

