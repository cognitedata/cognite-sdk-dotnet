// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Relationships Vertex property.
    /// </summary>
    public class RelationshipVertexProperty
    {
        /// <summary>
        /// Key of the vertex.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value of the vertex.
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
