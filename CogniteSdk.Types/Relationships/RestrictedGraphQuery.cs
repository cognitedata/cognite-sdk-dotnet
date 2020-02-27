// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Relationships
{
    /// <summary>
    /// Dto to perform a graph query.
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
    }
}