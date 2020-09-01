// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Label used to represent edges and vertices.
    /// </summary>
    public class RelationshipLabelDto
    {
        /// <summary>
        /// The id of a label.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The label for the object.
        /// </summary>
        public string Label { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
