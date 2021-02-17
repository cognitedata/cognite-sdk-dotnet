// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for errors in a GraphQL query.
    /// </summary>
    public class GraphQlError
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The path stored as an array of path segments.
        /// </summary>
        public List<string> Path { get; set; }

        /// <summary>
        /// The location of an error.
        /// </summary>
        public List<GraphQlLocation> Locations { get; set; }
    }
}
