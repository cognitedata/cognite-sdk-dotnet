// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Property data type
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PropertyTypeVariant
    {
        /// <summary>
        /// Text
        /// </summary>
        text,
        /// <summary>
        /// Boolean
        /// </summary>
        boolean,
        /// <summary>
        /// 32 bit floating point number
        /// </summary>
        float32,
        /// <summary>
        /// 64 bit floating point number
        /// </summary>
        float64,
        /// <summary>
        /// 32 bit integer
        /// </summary>
        int32,
        /// <summary>
        /// 64 bit integer
        /// </summary>
        int64,
        /// <summary>
        /// Timestamp with timezone
        /// </summary>
        timestamp,
        /// <summary>
        /// Date
        /// </summary>
        date,
        /// <summary>
        /// Arbitrary JSON
        /// </summary>
        json,
        /// <summary>
        /// Direct node relation
        /// </summary>
        direct,
    }

    /// <summary>
    /// Base class for property types
    /// </summary>
    public abstract class BasePropertyType
    {
        /// <summary>
        /// Property type
        /// </summary>
        public PropertySourceType Type { get; set; }
    }

    /// <summary>
    /// Text property type
    /// </summary>
    public class TextPropertyType : BasePropertyType
    {
        /// <summary>
        /// Whether this property is a list.
        /// </summary>
        public bool List { get; set; }
        /// <summary>
        /// Text collation.
        /// </summary>
        public string Collation { get; set; }
    }

    /// <summary>
    /// Primitive property type.
    /// </summary>
    public class PrimitivePropertyType : BasePropertyType
    {
        /// <summary>
        /// Whether this property is a list.
        /// </summary>
        public bool List { get; set; }
    }

    /// <summary>
    /// Direct relation property type.
    /// </summary>
    public class DirectRelationPropertyType : BasePropertyType
    {
        /// <summary>
        /// The (optional) required type for the node the direct relation points to.
        /// If specified, the node must exist before the direct relation is referenced.
        /// If no container specification is used, the node will be auto created with the
        /// built-in node container type, and it does not explicitly have to be created
        /// before the node that references it.
        /// </summary>
        public ContainerIdentifier Container { get; set; }

        /// <summary>
        /// Hint showing a view what the direct relation points to.
        /// Only valid when retrieving views.
        /// </summary>
        public ViewIdentifier Source { get; set; }
    }

    /// <summary>
    /// JsonConverter for property type variants
    /// </summary>
    public class PropertyTypeConverter : JsonConverter<BasePropertyType>
    {
        /// <inheritdoc />
        public override BasePropertyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var typeProp = document.RootElement.GetProperty("type").GetString();
            if (!Enum.TryParse<PropertyTypeVariant>(typeProp, true, out var type))
            {
                return null;
            }
            switch (type)
            {
                case PropertyTypeVariant.text:
                    return document.Deserialize<TextPropertyType>(options);
                case PropertyTypeVariant.direct:
                    return document.Deserialize<DirectRelationPropertyType>(options);
                default:
                    return document.Deserialize<PrimitivePropertyType>(options);
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BasePropertyType value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
