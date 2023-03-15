// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta.DataModels
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
        public PropertyTypeVariant Type { get; set; }

        /// <summary>
        /// Get a text property type
        /// </summary>
        /// <param name="list">True if this is a text[]</param>
        /// <param name="collation">Optional text collation</param>
        /// <returns></returns>
        public static TextPropertyType Text(bool list = false, string collation = null)
        {
            return new TextPropertyType
            {
                Type = PropertyTypeVariant.text,
                List = list,
                Collation = collation
            };
        }

        /// <summary>
        /// Get a primitive property type
        /// </summary>
        /// <param name="type">Property type</param>
        /// <param name="list">True if this is an array, not valid for direct relations</param>
        /// <returns></returns>
        public static BasePropertyType Create(PropertyTypeVariant type, bool list = false)
        {
            if (type == PropertyTypeVariant.text)
            {
                return Text(list);
            }
            else if (type == PropertyTypeVariant.direct)
            {
                return Direct();
            }
            else
            {
                return new PrimitivePropertyType
                {
                    Type = type,
                    List = list,
                };
            }
        }

        /// <summary>
        /// Get a direct relation property type
        /// </summary>
        /// <param name="container">Optional required type for the node this direct relation points to.</param>
        /// <returns></returns>
        public static DirectRelationPropertyType Direct(ContainerIdentifier container = null)
        {
            return new DirectRelationPropertyType
            {
                Container = container,
                Type = PropertyTypeVariant.direct
            };
        }
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
    public class PropertyTypeConverter : IntTaggedUnionConverter<BasePropertyType, PropertyTypeVariant>
    {
        /// <inheritdoc />
        protected override string TypePropertyName => "type";

        /// <inheritdoc />
        protected override BasePropertyType DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, PropertyTypeVariant type)
        {
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
    }
}
