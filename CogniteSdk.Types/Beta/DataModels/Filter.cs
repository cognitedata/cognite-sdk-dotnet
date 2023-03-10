// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Base interface for data model storage filters.
    /// </summary>
    public interface IDMSFilter
    {
    }

    /// <summary>
    /// Json converter for data model storage filters.
    /// </summary>
    public class DmsFilterConverter : JsonConverter<IDMSFilter>
    {
        /// <inheritdoc />
        public override IDMSFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                if (!reader.TrySkip()) throw new JsonException("Could not skip empty filter");
                return null;
            }
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject) return null;
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                if (!reader.TrySkip()) throw new JsonException("Could not skip empty filter");
                return null;
            }
            var propertyName = reader.GetString();
            reader.Read();
            IDMSFilter filter = null;
            switch (propertyName)
            {
                case "and": filter = new AndFilter { And = JsonSerializer.Deserialize<IEnumerable<IDMSFilter>>(ref reader, options) }; break;
                case "or": filter = new OrFilter { Or = JsonSerializer.Deserialize<IEnumerable<IDMSFilter>>(ref reader, options) }; break;
                case "not": filter = new NotFilter { Not = JsonSerializer.Deserialize<IDMSFilter>(ref reader, options) }; break;
                case "containsAll": filter = JsonSerializer.Deserialize<ContainsAllFilter>(ref reader, options); break;
                case "containsAny": filter = JsonSerializer.Deserialize<ContainsAnyFilter>(ref reader, options); break;
                case "equals": filter = JsonSerializer.Deserialize<EqualsFilter>(ref reader, options); break;
                case "exists": filter = JsonSerializer.Deserialize<ExistsFilter>(ref reader, options); break;
                case "in": filter = JsonSerializer.Deserialize<InFilter>(ref reader, options); break;
                case "hasData": filter = JsonSerializer.Deserialize<HasDataFilter>(ref reader, options); break;
                case "matchAll": filter = JsonSerializer.Deserialize<MatchAllFilter>(ref reader, options); break;
                case "prefix": filter = JsonSerializer.Deserialize<PrefixFilter>(ref reader, options); break;
                case "range": filter = JsonSerializer.Deserialize<RangeFilter>(ref reader, options); break;
                case "nested": filter = JsonSerializer.Deserialize<NestedFilter>(ref reader, options); break;
            }
            if (filter == null)
            {
                if (!reader.TrySkip()) throw new JsonException("Failed to skip unknown filter");
            }
            reader.Read();
            return filter;
        }
        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IDMSFilter value, JsonSerializerOptions options)
        {
            if (value is ICompositeDMSFilter)
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
            else
            {
                writer.WriteStartObject();

                var typeName = value.GetType().Name;
                var propertyName = (char.ToLower(typeName[0]) + typeName.Substring(1)).Replace("Filter", "");
                writer.WritePropertyName(propertyName);

                JsonSerializer.Serialize(writer, value, value.GetType(), options);

                writer.WriteEndObject();
            }
        }
    }

    /// <summary>
    /// Converter for DMSFilterValue
    /// </summary>
    public class DmsFilterValueConverter : JsonConverter<IDMSValue>
    {
        /// <inheritdoc />
        public override IDMSValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return new RawPropertyValue<double>(JsonSerializer.Deserialize<double>(ref reader, options));
            }
            else if (reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.True)
            {
                return new RawPropertyValue<bool>(JsonSerializer.Deserialize<bool>(ref reader, options));
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return new RawPropertyValue<string>(JsonSerializer.Deserialize<string>(ref reader, options));
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var res = JsonSerializer.Deserialize<IEnumerable<IDMSValue>>(ref reader, options);
                var types = res.Select(x => x.GetType().GenericTypeArguments[0]).Distinct().ToList();
                if (types.Count() > 1) throw new JsonException("Contents of DMSFilterValue as array must all be same type");
                var type = types.First();

                var arrayType = type.MakeArrayType();
                var resultType = typeof(RawPropertyValue<>).MakeGenericType(arrayType);
                var result = Activator.CreateInstance(resultType);
                resultType.GetProperty("Value").SetValue(result, res);
                return (IDMSValue)result;
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // This can (unfortunately) be either an _arbitrary_ json object, or one of the two special value types.
                // Since we really have no good way to distinguish between them, we just store it as JsonElement.
                // It is an edge case either way.
                var res = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                return new RawPropertyValue<JsonElement>();
            }
            else
            {
                throw new JsonException("Unexpected filter value type");
            }

        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IDMSValue value, JsonSerializerOptions options)
        {
            if (value is IRawPropertyValue)
            {
                var innerValue = value.GetType().GetProperty("Value").GetValue(value);
                if (innerValue == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, innerValue, innerValue.GetType(), options);
                }
            }
            else
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
        }
    }

    /// <summary>
    /// Interface used for serializing the DMSValue types.
    /// </summary>
    public interface IDMSValue { }

    /// <summary>
    /// Non-generic base class of RawPropertyValue[T].
    /// </summary>
    public interface IRawPropertyValue : IDMSValue { }

    /// <summary>
    /// A value 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RawPropertyValue<T> : IRawPropertyValue
    {
        /// <summary>
        /// Value of this filter. Should be string, double, boolean, or an array of these.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public RawPropertyValue() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value, should be string, double, boolean, or an array of these</param>
        public RawPropertyValue(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// A parametrized FDM filter value.
    /// </summary>
    public class ParameterizedPropertyValue : IDMSValue
    {
        /// <summary>
        /// Parameter
        /// </summary>
        public string Parameter { get; set; }
    }

    /// <summary>
    /// A referenced FDM property value.
    /// </summary>
    public class ReferencedPropertyValue : IDMSValue
    {
        /// <summary>
        /// The referenced property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Interface indicating that this is a composite filter that should not be nested in its own name.
    /// Necessary because a type cannot also be a list.
    /// </summary>
    public interface ICompositeDMSFilter { }

    /// <summary>
    /// Filter AND-ing together a list of other filters.
    /// </summary>
    public class AndFilter : IDMSFilter, ICompositeDMSFilter
    {
        /// <summary>
        /// List of clauses to be AND-ed together.
        /// </summary>
        public IEnumerable<IDMSFilter> And { get; set; }
    }

    /// <summary>
    /// Filter OR-ing together a list of other filters.
    /// </summary>
    public class OrFilter : IDMSFilter, ICompositeDMSFilter
    {
        /// <summary>
        /// List of clauses to be OR-ed together.
        /// </summary>
        public IEnumerable<IDMSFilter> Or { get; set; }
    }

    /// <summary>
    /// Filter inverting another filter.
    /// </summary>
    public class NotFilter : IDMSFilter, ICompositeDMSFilter
    {
        /// <summary>
        /// Clause to be inverted.
        /// </summary>
        public IDMSFilter Not { get; set; }
    }

    /// <summary>
    /// Filter for specifying that a property must contain everything in "Values".
    /// </summary>
    public class ContainsAllFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// List of required values.
        /// </summary>
        public IEnumerable<IDMSValue> Values { get; set; }
    }

    /// <summary>
    /// Filter for specifying that a property must contain any element in "Values".
    /// </summary>
    public class ContainsAnyFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// List of values.
        /// </summary>
        public IEnumerable<IDMSValue> Values { get; set; }
    }

    /// <summary>
    /// Filter for specifying that a property must be equal to "Value".
    /// </summary>
    public class EqualsFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// Value of property.
        /// </summary>
        public IDMSValue Value { get; set; }
    }

    /// <summary>
    /// Filter for specifying that a property must exist.
    /// </summary>
    public class ExistsFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Filter for specifying that a property must be in "Values".
    /// </summary>
    public class InFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// List of values.
        /// </summary>
        public IEnumerable<IDMSValue> Values { get; set; }
    }

    /// <summary>
    /// Filter for specifying that a node must have values from all the given models.
    /// </summary>
    public class HasDataFilter : IDMSFilter
    {
        /// <summary>
        /// List of models.
        /// </summary>
        public IEnumerable<FDMIdentifier> Models { get; set; }
    }

    /// <summary>
    /// Empty filter that matches all nodes.
    /// </summary>
    public class MatchAllFilter : IDMSFilter
    {
    }

    /// <summary>
    /// Filter that matches if the given property is a string prefixed by the given value.
    /// </summary>
    public class PrefixFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// Prefix
        /// </summary>
        public IDMSValue Value { get; set; }
    }

    /// <summary>
    /// Filter for ranges of data. Only one each of (GreaterThan, GreaterThanEqual) and (LessThan, LessThanEqual)
    /// may be specified.
    /// </summary>
    public class RangeFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }

        /// <summary>
        /// Value must be greater than this
        /// </summary>
        [JsonPropertyName("gt")]
        public IDMSValue GreaterThan { get; set; }

        /// <summary>
        /// Value must be greater than or equal to this
        /// </summary>
        [JsonPropertyName("gte")]
        public IDMSValue GreaterThanEqual { get; set; }

        /// <summary>
        /// Value must be less than this
        /// </summary>
        [JsonPropertyName("lt")]
        public IDMSValue LessThan { get; set; }

        /// <summary>
        /// Value must be less than or equal to this
        /// </summary>
        [JsonPropertyName("lte")]
        public IDMSValue LessThanEqual { get; set; }
    }

    /// <summary>
    /// Filter on nodes referenced through direct_relation.
    /// </summary>
    public class NestedFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property containing the direct_relation.
        /// </summary>
        public IEnumerable<string> Scope { get; set; }

        /// <summary>
        /// Filter on the referenced node.
        /// </summary>
        public IDMSFilter Filter { get; set; }
    }

    public class OverlapsFilter : IDMSFilter
    {
        /// <summary>
        /// List of strings defining the property that marks the start of the range.
        /// </summary>
        public IEnumerable<string> StartProperty { get; set; }
        /// <summary>
        /// List of strings defining the property that marks the end of the range.
        /// </summary>
        public IEnumerable<string> EndProperty { get; set; }
        /// <summary>
        /// Value must be greater than this
        /// </summary>
        [JsonPropertyName("gt")]
        public IDMSValue GreaterThan { get; set; }

        /// <summary>
        /// Value must be greater than or equal to this
        /// </summary>
        [JsonPropertyName("gte")]
        public IDMSValue GreaterThanEqual { get; set; }
        /// <summary>
        /// Value must be less than this
        /// </summary>
        [JsonPropertyName("lt")]
        public IDMSValue LessThan { get; set; }

        /// <summary>
        /// Value must be less than or equal to this
        /// </summary>
        [JsonPropertyName("lte")]
        public IDMSValue LessThanEqual { get; set; }
    }
}