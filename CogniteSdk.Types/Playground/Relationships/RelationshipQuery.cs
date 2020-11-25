// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Playground
{
    /// <summary>
    /// Query parameters for list relationships.
    /// </summary>
    public class RelationshipQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on relationships.
        /// </summary>
        public RelationshipFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
