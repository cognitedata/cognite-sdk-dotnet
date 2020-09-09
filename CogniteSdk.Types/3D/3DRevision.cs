// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevision read class.
    /// </summary>
    public class ThreeDRevision
    {
        /// <summary>
        /// The Id of the 3D revision.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The file id.
        /// </summary>
        public long FileId { get; set; }
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
        public string Status { get; set; }
        /// <summary>
        /// The 3D file ID of a thumbnail for the revision. Use /3d/files/{id} to retrieve the file.
        /// </summary>
        public long ThumbnailThreeDFileId { get; set; }
        /// <summary>
        /// The URL of a thumbnail for the revision.
        /// </summary>
        public string ThumbnailUrl { get; set; }
        /// <summary>
        /// The number of asset mappings for this revision.
        /// </summary>
        public long AssetMappingCount { get; set; }

        /// <summary>
        /// The creation time of the 3D revision.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is ThreeDRevision dto &&
                   Id == dto.Id;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

