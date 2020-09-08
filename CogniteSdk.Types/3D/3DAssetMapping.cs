// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDAssetMapping read class.
    /// </summary>
    public class ThreeDAssetMapping
    {
        /// <summary>
        /// The ID of the 3D Node.
        /// </summary>
        public long NodeId { get; set; }
        /// <summary>
        /// The ID of the associated asset (Cognite's Assets API).
        /// </summary>
        public long AssetId { get; set; }
        /// <summary>
        /// A number describing the position of this node in the 3D
        /// hierarchy, starting from 0. The tree is traversed in a depth-first order.
        /// </summary>
        public long TreeIndex { get; set; }
        /// <summary>
        /// The number of nodes in the subtree of this
        /// node (this number included the node itself).
        /// </summary>
        public long SubtreeSize { get; set; }
        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is ThreeDAssetMapping mapping &&
                   NodeId == mapping.NodeId &&
                   AssetId == mapping.AssetId &&
                   TreeIndex == mapping.TreeIndex &&
                   SubtreeSize == mapping.SubtreeSize;
        }
        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = -449695491;
            hashCode = hashCode * -1521134295 + NodeId.GetHashCode();
            hashCode = hashCode * -1521134295 + AssetId.GetHashCode();
            hashCode = hashCode * -1521134295 + TreeIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + SubtreeSize.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

