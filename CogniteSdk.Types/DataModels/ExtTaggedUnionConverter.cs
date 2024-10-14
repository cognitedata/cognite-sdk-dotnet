// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Base json converter for externally tagged type unions.
    /// I.e. on the form
    /// 
    /// "myVariant": {
    ///     "contents": 123,
    ///     "contents2": 321
    /// }
    /// 
    /// converts to class MyVariant {
    ///     public int Contents { get; set; }
    ///     public int Contents2 { get; set; }
    /// }
    /// </summary>
    /// <typeparam name="T">Base class for converted types</typeparam>
    public abstract class ExtTaggedUnionConverter<T> : JsonConverter<T> where T : class
    {
        /// <summary>
        /// Deserialize into type <typeparamref name="T"/> given an external property name.
        /// Return null if it doesn't match.
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="options">Json serializer options</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Null or a deserialized value</returns>
        protected abstract T DeserializeFromPropertyName(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName);

        /// <summary>
        /// Suffix on union types. This is removed from the type name before writing.
        /// The type name should be on the form "[Tag][TypeSuffix]". The first letter is converted to lowercase.
        /// </summary>
        protected abstract string TypeSuffix { get; }

        /// <inheritdoc />
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                if (!reader.TrySkip()) throw new JsonException("Failed to skip empty union");
                return null;
            }
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject) return null;
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                if (!reader.TrySkip()) throw new JsonException("Failed to skip empty union");
                return null;
            }
            var propertyName = reader.GetString();
            reader.Read();
            var result = DeserializeFromPropertyName(ref reader, options, propertyName);
            if (result == null)
            {
                if (!reader.TrySkip()) throw new JsonException($"Failed to skip unknown union {propertyName}");
            }
            reader.Read();
            return result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var typeName = value.GetType().Name;
            var propertyName = (char.ToLower(typeName[0]) + typeName.Substring(1)).Replace(TypeSuffix, "");
            writer.WritePropertyName(propertyName);

            JsonSerializer.Serialize(writer, value, value.GetType(), options);

            writer.WriteEndObject();
        }
    }
}
