// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for location in a GraphQL query.
    /// </summary>
    public class GraphQlLocation
    {
        /// <summary>
        /// Represents a one-indexed line number into the query.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Represents a one-indexed column number into the query.
        /// </summary>        
        public int Column { get; set; }
    }
}
