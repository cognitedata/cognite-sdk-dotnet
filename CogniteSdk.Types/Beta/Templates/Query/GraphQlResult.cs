﻿// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{

    /// <summary>
    /// Class for storing a GraphQL query result.
    /// </summary>
    public class GraphQlResult<T>
    {
        /// <summary>
        /// The GraphQL data result.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// The GraphQL errors if any.
        /// </summary>
        public List<GraphQlError> Errors { get; set; }
    }
}
