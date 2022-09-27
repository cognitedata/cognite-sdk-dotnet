// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Indexes for properties in a model.
    /// </summary>
    public class ModelIndexes
    {
        /// <summary>
        /// A collection of B-tree indexes.
        /// </summary>
        [JsonPropertyName("btreeIndex")]
        public Dictionary<string, ModelIndexProperty> BTreeIndex { get; set; }
    }

    /// <summary>
    /// A property on a model index.
    /// </summary>
    public class ModelIndexProperty
    {
        /// <summary>
        /// List of properties this index consists of
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
    }
}
