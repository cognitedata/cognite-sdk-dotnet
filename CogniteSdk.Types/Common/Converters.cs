// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Converts multiple values values from JSON to LongValue, DoubleValue or StringValue.
    /// </summary>
    public class MultiValueConverter : JsonConverter<MultiValue>
    {
        /// <summary>
        /// Creates MultiValue values from the JSON input.
        /// </summary>
        /// <returns>The MultiValue. Either LongValue, DoubleValue or StringValue.</returns>
        public override MultiValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return MultiValue.Create(reader.GetString());
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var longNumber))
                    {
                        return MultiValue.Create(longNumber);
                    }

                    return MultiValue.Create(reader.GetDouble());
                case JsonTokenType.Null:
                    return MultiValue.Create();
                default:
                    throw new JsonException($"Unable to parse value of type: {reader.TokenType}");
            }
        }

        /// <summary>
        /// Writes MultiValue values to JSON numbers or strings.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, MultiValue value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            switch (value)
            {
                case MultiValue.String s:
                    writer.WriteStringValue(s.Value);
                    break;
                case MultiValue.Double d:
                    writer.WriteNumberValue(d.Value);
                    break;
                case MultiValue.Long l:
                    writer.WriteNumberValue(l.Value);
                    break;
                case MultiValue.Null n:
                    writer.WriteNullValue();
                    break;
                default:
                    throw new ArgumentException($"Unknown MultiValue: {value}");
            }
        }
    }

    /// <summary>
    /// Converts JSON to .NET type
    /// </summary>
    public class ObjectToDictionaryJsonConverter : JsonConverter<Dictionary<string, object>>
    {
        /// <summary>
        /// Reads JSON into a nested dictionary
        /// </summary>
        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, must be StartObject");
            }

            var dictionary = new Dictionary<string, object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"JsonTokenType must be of type PropertyName, was {reader.TokenType}");
                }

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new JsonException("Failed to get property name");
                }

                reader.Read();

                dictionary.Add(propertyName, ReadValue(ref reader, options));
            }

            return dictionary;
        }

        /// <summary>
        /// Writing a dictionary of objects to string is not supported.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        private object ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var number))
                    {
                        return number;
                    }
                    return reader.GetDouble();
                case JsonTokenType.StartObject:
                    return Read(ref reader, null, options);
                case JsonTokenType.StartArray:
                    var list = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(ReadValue(ref reader, options));
                    }
                    return list;
                default:
                    throw new JsonException($"'{reader.TokenType}' is not supported");
            }
        }
    }

}
