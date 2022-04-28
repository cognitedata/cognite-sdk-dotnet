// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for doing a GraphQL query to a domain.
    /// </summary>
    public class GraphQlQuery
    {
        /// <summary>
        /// The GraphQL query to run against a domain.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Variables to query.
        /// </summary>
        public JsonElement? Variables { get; set; }

        /// <summary>
        /// Name of operation
        /// </summary>
        public string OperationName { get; set; }
    }
}
