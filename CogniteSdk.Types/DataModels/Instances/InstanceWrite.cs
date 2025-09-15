// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.DataModels
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
        /// Instance type as direct relation to another node. Requried for edges.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }
        /// <summary>
        /// List of source properties to write. The properties are from the
        /// views/containers that make up this instance.
        /// </summary>
        public IEnumerable<InstanceData> Sources { get; set; }
    }

    /// <summary>
    /// Ingest a node.
    /// </summary>
    public class NodeWrite : BaseInstanceWrite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NodeWrite()
        {
            InstanceType = InstanceType.node;
        }
    }

    /// <summary>
    /// Ingest an edge.
    /// </summary>
    public class EdgeWrite : BaseInstanceWrite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EdgeWrite()
        {
            InstanceType = InstanceType.edge;
        }
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
    /// Use <see cref="InstanceData{T}"/> to set values, you can use
    /// your own DTOs, dynamic, or <see cref="StandardInstanceWriteData"/> as contents,
    /// for example.
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
    /// Simple payload for an instance.
    /// </summary>
    public class StandardInstanceWriteData : Dictionary<string, IDMSValue> { }

    /// <summary>
    /// JsonConverter for instance write variants
    /// </summary>
    public class InstanceWriteConverter : IntTaggedUnionConverter<BaseInstanceWrite, InstanceType>
    {
        /// <inheritdoc />
        protected override string TypePropertyName => "instanceType";

        /// <inheritdoc />
        protected override BaseInstanceWrite DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, InstanceType type)
        {
            switch (type)
            {
                case InstanceType.node:
                    return document.Deserialize<NodeWrite>(options);
                default:
                    return document.Deserialize<EdgeWrite>(options);
            }
        }
    }

    /// <summary>
    /// JsonConverter for instance data.
    /// </summary>
    public class InstanceDataConverter : JsonConverter<InstanceData>
    {
        /// <inheritdoc />
        public override InstanceData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<InstanceData<StandardInstanceWriteData>>(ref reader, options);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, InstanceData value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    /// <summary>
    /// Create a list of nodes or edges.
    /// </summary>
    public class InstanceWriteRequest : ItemsWithoutCursor<BaseInstanceWrite>
    {
        /// <summary>
        /// Should we create missing target nodes of direct relations?
        /// If the target-container constraint has been specified for a direct relation,
        /// the target node cannot be auto-created. If you want to point direct relations
        /// to a space where you have only read access, this option must be set to false.
        /// </summary>
        public bool AutoCreateDirectRelations { get; set; } = true;
        /// <summary>
        /// Auto create missing start nodes for edges.
        /// </summary>
        public bool AutoCreateStartNodes { get; set; }
        /// <summary>
        /// Auto create missing end nodes for edges.
        /// </summary>
        public bool AutoCreateEndNodes { get; set; }
        /// <summary>
        /// If existingVersion is specified on any of the nodes/edges in the input, the default behavior is
        /// that the entire ingestion fails. If skipOnVersionConflict is set to true, items with version conflicts
        /// will be skipped instead.
        /// </summary>
        public bool SkipOnVersionConflict { get; set; }
        /// <summary>
        /// True to replace existing values on conflict, false to merge, only adding
        /// properties that did not previously exist.
        /// </summary>
        public bool Replace { get; set; }
    }
}
