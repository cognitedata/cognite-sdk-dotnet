// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CogniteSdk.Beta
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

    /// <summary>
    /// JsonConverter for container index variants
    /// </summary>
    public class ContainerIndexConverter : JsonConverter<BaseIndex>
    {
        /// <inheritdoc />
        public override BaseIndex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var typeProp = document.RootElement.GetProperty("type").GetString();
            if (!Enum.TryParse<IndexType>(typeProp, true, out var type))
            {
                return null;
            }
            switch (type)
            {
                case IndexType.btree:
                    return document.Deserialize<BTreeIndex>(options);
            }
            return null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BaseIndex value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
