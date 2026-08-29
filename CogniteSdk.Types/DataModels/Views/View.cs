// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Enumeration of the possible uses for a flexible data models type.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UsedFor
    {
        /// <summary>
        /// Applies to nodes only
        /// </summary>
        node,
        /// <summary>
        /// Applies to edges only
        /// </summary>
        edge,
        /// <summary>
        /// Applies to both nodes and edges, but not records
        /// </summary>
        all,
        /// <summary>
        /// Applies to records only
        /// </summary>
        record,
    }
    /// <summary>
    /// Possible directions of a connection
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConnectionDirection
    {
        /// <summary>
        /// Connections pointing outwards.
        /// </summary>
        outwards,
        /// <summary>
        /// Connections pointing inwards.
        /// </summary>
        inwards,
    }
    /// <summary>
    /// A flexible data models view.
    /// </summary>
    public class View : IViewDefinitionOrReference
    {
        /// <summary>
        /// External ID uniquely identifying this view.
        /// The values Query, Mutation, Subscription, String,
        /// Int32, Int64, Int, Float32, Float64, Float, Timestamp, JSONObject,
        /// Date, Numeric, Boolean, and PageInfo are reserved.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Id of the space that the view belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Human readable name for the view.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the view.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A complex filter the contents of the view must match.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// References to views which this view will inherit from.
        /// 
        /// Note: The order is significant. It is used to deduce the priority when
        /// duplicate property references are encountered.
        /// 
        /// If you do not specify a view version, the most recent version available will be used.
        /// </summary>
        public IEnumerable<ViewIdentifier> Implements { get; set; }
        /// <summary>
        /// Version of the view. Must match the regular expression
        /// ^[a-zA-Z0-9]([a-zA-Z0-9_-]{0,41}[a-zA-Z0-9])?$
        /// </summary>
        public string Version { get; set; }
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
        /// Does the view support write operations?
        /// You can write to a view if the view maps all non-nullable properties, and the view has no
        /// relations (filters).
        /// </summary>
        public bool Writable { get; set; }
        /// <summary>
        /// Does the view support query operations?
        /// </summary>
        public bool Queryable { get; set; }
        /// <summary>
        /// Whether this view applies to nodes, edges, both nodes and edges, or records.
        /// </summary>
        public UsedFor UsedFor { get; set; }
        /// <summary>
        /// Is this a global view.
        /// </summary>
        public bool IsGlobal { get; set; }
        /// <summary>
        /// List of containers with properties mapped by this view.
        /// </summary>
        public IEnumerable<ContainerIdentifier> MappedContainers { get; set; }
        /// <summary>
        /// List of properties and relations included in this view.
        /// </summary>
        public Dictionary<string, IViewProperty> Properties { get; set; }
    }

    /// <summary>
    /// JsonConverter for View and its subtypes (such as RecordView)
    /// </summary>
    public class ViewConverter : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(View);
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var innerOptions = new JsonSerializerOptions(options);
            for (int i = innerOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (innerOptions.Converters[i] is ViewConverter)
                    innerOptions.Converters.RemoveAt(i);
            }
            return new ViewJsonConverter(innerOptions);
        }

        private sealed class ViewJsonConverter : JsonConverter<View>
        {
            private readonly JsonSerializerOptions _innerOptions;

            public ViewJsonConverter(JsonSerializerOptions innerOptions)
            {
                _innerOptions = innerOptions;
            }

            public override View Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                    throw new JsonException("Expected JSON object for View");

                if (root.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "view"
                    && !root.TryGetProperty("properties", out _)
                    && !root.TryGetProperty("createdTime", out _))
                    throw new JsonException("JSON object represents a ViewIdentifier reference, not a full View definition");

                bool isRecordView = false;
                if (root.TryGetProperty("usedFor", out var usedForProp) && usedForProp.GetString() == "record")
                    isRecordView = true;
                else if (root.TryGetProperty("streamId", out _))
                    isRecordView = true;

                var rawText = root.GetRawText();
                if (isRecordView)
                    return JsonSerializer.Deserialize<RecordView>(rawText, _innerOptions);
                else
                    return JsonSerializer.Deserialize<View>(rawText, _innerOptions);
            }

            public override void Write(Utf8JsonWriter writer, View value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), _innerOptions);
            }
        }
    }

    /// <summary>
    /// Interface for possible view property types.
    /// </summary>
    public interface IViewProperty { }

    /// <summary>
    /// Description of a view property.
    /// </summary>
    public class ViewPropertyDefinition : IViewProperty
    {
        /// <summary>
        /// Whether this property can be set to null.
        /// </summary>
        public bool Nullable { get; set; } = true;
        /// <summary>
        /// Whether to auto increment the property based on the highest current max value.
        /// Only applicable to properties of type int32 or int64.
        /// </summary>
        public bool AutoIncrement { get; set; }
        /// <summary>
        /// Optional default value for the property.
        /// </summary>
        public IDMSValue DefaultValue { get; set; }
        /// <summary>
        /// Description of the property.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Human readable property name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The data-type to use for the property.
        /// </summary>
        public BasePropertyType Type { get; set; }
        /// <summary>
        /// Reference to an existing container.
        /// </summary>
        public ContainerIdentifier Container { get; set; }
        /// <summary>
        /// The unique identifier, in the referenced container, for the property to map.
        /// </summary>
        public string ContainerPropertyIdentifier { get; set; }
    }

    /// <summary>
    /// Description of a view connection.
    /// </summary>
    public class ConnectionDefinition : IViewProperty, ICreateViewProperty
    {
        /// <summary>
        /// Description of the property.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Human readable property name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Reference to the direct relation.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }
        /// <summary>
        /// Reference to a view.
        /// </summary>
        public ViewIdentifier Source { get; set; }
        /// <summary>
        /// Direction of the connection.
        /// </summary>
        public ConnectionDirection Direction { get; set; }
    }

    /// <summary>
    /// Json converter for view property types.
    /// </summary>
    public class ViewPropertyConverter : UntaggedUnionConverter<IViewProperty>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewPropertyConverter() : base(new[]
        {
            typeof(ViewPropertyDefinition), typeof(ConnectionDefinition)
        })
        {
        }
    }
}
