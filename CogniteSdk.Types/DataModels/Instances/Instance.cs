// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Base type for read instances.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseInstance<T>
    {
        /// <summary>
        /// Type of instance.
        /// </summary>
        public InstanceType InstanceType { get; set; }
        /// <summary>
        /// Instance type as direct relation to another node. Required for edges.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }
        /// <summary>
        /// Current version of this instance.
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Id of the space that the node belongs to. This space-id cannot be updated.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Unique identifier for the node.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Generic properties type.
        /// This is set of nested maps from space,
        /// view/container, and property name to property value.
        /// </summary>
        public T Properties { get; set; }
        /// <summary>
        /// Time when this view was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this view was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// Timestamp this node was soft deleted. This is only present in sync results.
        /// </summary>
        public long? DeletedTime { get; set; }
    }

    /// <summary>
    /// A retrieved node.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Node<T> : BaseInstance<T> { }

    /// <summary>
    /// A retrieved edge.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Edge<T> : BaseInstance<T>
    {
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
    /// Non-generic instance data.
    /// This is a map from space, view/container id, and property to value.
    /// </summary>
    public class StandardInstanceData : Dictionary<string, Dictionary<string, Dictionary<string, IDMSValue>>>
    {
    }

    /// <summary>
    /// JsonConverterFactory for instance read variants.
    /// </summary>
    public class InstanceConverterFactory : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType) return false;
            return typeToConvert.GetGenericTypeDefinition() == typeof(BaseInstance<>);
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var innerType = typeToConvert.GetGenericArguments()[0];
            return (JsonConverter)Activator.CreateInstance(
                typeof(InstanceConverter<>).MakeGenericType(innerType));
        }
    }

    /// <summary>
    /// JsonConverter for instance read variants.
    /// </summary>
    /// <typeparam name="T">Inner type of properties</typeparam>
    public class InstanceConverter<T> : IntTaggedUnionConverter<BaseInstance<T>, InstanceType>
    {
        /// <inheritdoc />
        protected override string TypePropertyName => "instanceType";

        /// <inheritdoc />
        protected override BaseInstance<T> DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, InstanceType type)
        {
            switch (type)
            {
                case InstanceType.node:
                    return document.Deserialize<Node<T>>(options);
                default:
                    return document.Deserialize<Edge<T>>(options);
            }
        }
    }
}
