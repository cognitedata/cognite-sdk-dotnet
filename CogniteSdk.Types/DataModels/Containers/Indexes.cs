// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Index type
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IndexType
    {
        /// <summary>
        /// B-tree index
        /// </summary>
        btree
    }

    /// <summary>
    /// Base class for index types.
    /// </summary>
    public abstract class BaseIndex
    {
        /// <summary>
        /// Type of index.
        /// </summary>
        public IndexType IndexType { get; set; }
    }

    /// <summary>
    /// A B-Tree index.
    /// </summary>
    public class BTreeIndex : BaseIndex
    {
        /// <summary>
        /// List of properties to define the index across.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
    }

    // New index types are likely to be added. This maintains compatibility with added variants,
    // just deserializing them to null.

    /// <summary>
    /// JsonConverter for container index variants
    /// </summary>
    public class ContainerIndexConverter : IntTaggedUnionConverter<BaseIndex, IndexType>
    {
        /// <inheritdoc />
        protected override string TypePropertyName => "indexType";

        /// <inheritdoc />
        protected override BaseIndex DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, IndexType type)
        {
            return document.Deserialize<BTreeIndex>(options);
        }
    }
}
