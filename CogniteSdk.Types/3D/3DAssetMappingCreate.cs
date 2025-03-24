// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDAssetMappingCreate class.
    /// </summary>
    public class ThreeDAssetMappingCreate
    {
        /// <summary>
        /// The ID of the 3D Node.
        /// </summary>
        public long NodeId { get; set; }

        /// <summary>
        /// The ID of the associated asset (Cognite's Assets API).
        /// </summary>
        public long AssetId { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is ThreeDAssetMappingCreate create &&
                   NodeId == create.NodeId &&
                   AssetId == create.AssetId;
        }
        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = -814975570;
            hashCode = hashCode * -1521134295 + NodeId.GetHashCode();
            hashCode = hashCode * -1521134295 + AssetId.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

