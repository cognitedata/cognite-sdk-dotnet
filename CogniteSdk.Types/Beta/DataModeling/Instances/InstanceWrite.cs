// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Enumeration of possible instance types.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InstanceType
    {
        /// <summary>
        /// Instance is a node
        /// </summary>
        node,
        /// <summary>
        /// Instance is an edge
        /// </summary>
        edge
    }

    /// <summary>
    /// Base class for ingestion of nodes or edges.
    /// </summary>
    public abstract class BaseInstanceWrite
    {
        /// <summary>
        /// Type of instance.
        /// </summary>
        public InstanceType InstanceType { get; set; }
        /// <summary>
        /// Fail the ingestion request if the node's version is greater than or equal to this value.
        /// If no existingVersion is specified, the ingestion will always overwrite any existing data.
        /// If existingVersion is set to0, the upsert will behave as an insert, so it will fail if
        /// the item already exists.
        /// If skipOnVersionConflict is set on the request, then the item will be skipped instead of
        /// failing.
        /// </summary>
        public int? ExistingVersion { get; set; }
        /// <summary>
        /// Id of the space that the node belongs to. This space-id cannot be updated.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Unique identifier for the node.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// List of source properties to write. The properties are from the
        /// views/containers that make up this instance.
        /// </summary>
        public IEnumerable<InstanceData> Sources { get; }
    }

    /// <summary>
    /// Ingest a node.
    /// </summary>
    public class NodeWrite : BaseInstanceWrite
    {
    }

    /// <summary>
    /// Ingest an edge.
    /// </summary>
    public class EdgeWrite : BaseInstanceWrite
    {
        /// <summary>
        /// Edge type as direct relation to another node.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }
        /// <summary>
        /// Start node of this edge.
        /// </summary>
        public DirectRelationIdentifier StartNode { get; set; }
        /// <summary>
        /// End node of this edge.
        /// </summary>
        public DirectRelationIdentifier EndNode { get; set; }
    }


    /// <summary>
    /// Base class for user-defined property value groups.
    /// </summary>
    public abstract class InstanceData
    {
        /// <summary>
        /// Source of the instance.
        /// </summary>
        public SourceIdentifier Source { get; set; }
    }

    /// <summary>
    /// A group of property values in an instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InstanceData<T> : InstanceData
    {
        /// <summary>
        /// Properties to ingest.
        /// </summary>
        public T Properties { get; set; }
    }

    /// <summary>
    /// JsonConverter for instance write variants
    /// </summary>
    public class InstanceWriteConverter : JsonConverter<BaseInstanceWrite>
    {
        /// <inheritdoc />
        public override BaseInstanceWrite Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var typeProp = document.RootElement.GetProperty("instanceType").GetString();
            if (!Enum.TryParse<InstanceType>(typeProp, true, out var type))
            {
                return null;
            }
            switch (type)
            {
                case InstanceType.node:
                    return document.Deserialize<NodeWrite>(options);
                case InstanceType.edge:
                    return document.Deserialize<EdgeWrite>(options);
            }
            return null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BaseInstanceWrite value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
