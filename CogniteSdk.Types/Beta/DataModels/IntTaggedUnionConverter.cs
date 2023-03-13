// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Base converter for internally tagged unions.
    /// I.e. on the form
    /// 
    /// {
    ///     "type": "myVariant",
    ///     "contents": 123
    /// }
    /// 
    /// converts to class MyVariant {
    ///     public MyVariantType Type { get; set; }
    ///     public int Contents { get; set; }
    /// }
    /// 
    /// </summary>
    /// <typeparam name="TResult">Base class for converted types</typeparam>
    /// <typeparam name="TEnum">Discriminator enum type</typeparam>
    public abstract class IntTaggedUnionConverter<TResult, TEnum> : JsonConverter<TResult>
        where TResult: class
        where TEnum: struct, Enum
    {
        /// <summary>
        /// Internal type property name.
        /// </summary>
        protected abstract string TypePropertyName { get; }

        /// <summary>
        /// Given enum, deserialize from reader.
        /// This must consume the input.
        /// </summary>
        /// <param name="document">Json document to read from</param>
        /// <param name="options">Json options</param>
        /// <param name="type">Internal tag</param>
        /// <returns></returns>
        protected abstract TResult DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, TEnum type);

        /// <inheritdoc />
        public override TResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var typeProp = document.RootElement.GetProperty(TypePropertyName).GetString();
            if (!Enum.TryParse<TEnum>(typeProp, true, out var type))
            {
                return null;
            }
            return DeserializeFromEnum(document, options, type);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TResult value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
