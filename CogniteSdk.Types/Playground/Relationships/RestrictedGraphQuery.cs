// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Playground
{
    /// <summary>
    /// Class to perform a graph query.
    /// </summary>
    public class RestrictedGraphQuery
    {
        /// <summary>
        /// Executable graph query, written in gremlin.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Filter on relationships.
        /// </summary>
        public RelationshipFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
