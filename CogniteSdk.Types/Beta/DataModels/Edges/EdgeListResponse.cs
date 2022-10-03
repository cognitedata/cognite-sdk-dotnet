// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Response to a query listing edges.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EdgeListResponse<T> : ItemsWithCursor<T> where T : BaseEdge
    {
        /// <summary>
        /// List of all properties in the retrieved model.
        /// </summary>
        public Dictionary<string, ModelProperty> ModelProperties { get; set; }
    }
}
