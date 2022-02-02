// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Schema of field returned from transformations preview.
    /// </summary>
    public class TransformationColumnType
    {
        /// <summary>
        /// Name of field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sql type of field.
        /// </summary>
        public string SqlType { get; set; }

        /// <summary>
        /// Json type of field.
        /// </summary>
        public TransformationJsonSchemaField Type { get; set; }

        /// <summary>
        /// True if this is inferred to be nullable.
        /// </summary>
        public bool Nullable { get; set; }
    }


    /// <summary>
    /// Part of Json schema structure for transformation schema inference.
    /// </summary>
    public class TransformationJsonSchemaField
    {
        /// <summary>
        /// Name of json type. "struct", "array", "long", "boolean", "string", "float".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// List of fields if this is a "struct".
        /// </summary>
        public IEnumerable<TransformationJsonSchemaField> Fields { get; set; }

        /// <summary>
        /// Name of field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if this field is nullable.
        /// </summary>
        public bool? Nullable { get; set; }

        /// <summary>
        /// True if this is an array that contains null values.
        /// </summary>
        public bool? ContainsNull { get; set; }

        /// <summary>
        /// Type of element if this is an array.
        /// </summary>
        public TransformationJsonSchemaField ElementType { get; set; }
    }

    /// <summary>
    /// Json converter for transformation json schema fields.
    /// </summary>
    public class TransformationSchemaConverter : JsonConverter<TransformationJsonSchemaField>
    {
        /// <summary>
        /// Read a transformation schema field from Json.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override TransformationJsonSchemaField Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.String)
            {
                return new TransformationJsonSchemaField { Type = reader.GetString() };
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                return JsonSerializer.Deserialize<TransformationJsonSchemaField>(ref reader, options);
            }
            else
            {
                if (!reader.TrySkip()) throw new JsonException("Failed to skip invalid type in transformation schema");
                return new TransformationJsonSchemaField();
            }
        }

        /// <summary>
        /// Write a transformation schema field to json.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, TransformationJsonSchemaField value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
