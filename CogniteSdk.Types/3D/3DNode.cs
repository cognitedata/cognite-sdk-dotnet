// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDNode read class.
    /// </summary>
    public class ThreeDNode
    {
        /// <summary>
        /// The Id of the 3D node.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The index of the node in the 3D model hierarchy, starting from 0.
        /// The tree is traversed in a depth-first order.
        /// </summary>
        public long TreeIndex { get; set; }
        /// <summary>
        /// The parent of the node, null if it is the root node.
        /// </summary>
        public long ParentId { get; set; }
        /// <summary>
        /// The depth of the node in the tree, starting from 0 at the root node.
        /// </summary>
        public long depth { get; set; }
        /// <summary>
        /// The number of descendants of the node, plus one (counting itself).
        /// </summary>
        public long SubtreeSize { get; set; }
        /// <summary>
        /// Properties extracted from 3D model, with property
        /// categories containing key/value string pairs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, Dictionary<string, string>> Properties { get; set; }
        /// <summary>
        /// The bounding box of the subtree with this sector as the root sector.
        /// Is null if there are no geometries in the subtree.
        /// </summary>
        public ThreeDBoundingBox BoundingBox { get; set; }

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
            return obj is ThreeDNode dto &&
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

