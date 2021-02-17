// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Beta
{

    /// <summary>
    /// Class for storing a GraphQL query result.
    /// </summary>
    public class GraphQlResult
    {
        /// <summary>
        /// The GraphQL data result.
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// The GraphQL errors if any.
        /// </summary>
        public List<GraphQlError> Errors { get; set; }
    }
}