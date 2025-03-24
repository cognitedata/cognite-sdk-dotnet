// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Value that is either a string or an integer.
    /// </summary>
    public abstract class StringOrInt
    {
        /// <summary>
        /// Create a StringOrInt from a string value.
        /// </summary>
        /// <param name="value">String value</param>
        public static StringOrInt Create(string value)
        {
            return new String(value);
        }

        /// <summary>
        /// Create a StringOrInt from an integer value.
        /// </summary>
        /// <param name="value">Integer value</param>
        public static StringOrInt Create(int value)
        {
            return new Int(value);
        }


        /// <summary>
        /// Instance of StringOrInt that is a string.
        /// </summary>
        public class String : StringOrInt
        {
            /// <summary>
            /// String value.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="value">String value</param>
            public String(string value)
            {
                Value = value;
            }
        }

        /// <summary>
        /// Instance of StringOrInt that is an integer.
        /// </summary>
        public class Int : StringOrInt
        {
            /// <summary>
            /// Integer value.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="value">Integer value</param>
            public Int(int value)
            {
                Value = value;
            }
        }
    }

    /// <summary>
    /// Json converter for StringOrInt.
    /// </summary>
    public class StringOrIntConverter : JsonConverter<StringOrInt>
    {
        /// <inheritdoc />
        public override StringOrInt Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return StringOrInt.Create(reader.GetString());
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var number))
                    {
                        return StringOrInt.Create(number);
                    }

                    throw new JsonException("Unable to parse value as integer");
                case JsonTokenType.Null:
                    return null;
                default:
                    throw new JsonException($"Unable to parse value of type: {reader.TokenType}");
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, StringOrInt value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            switch (value)
            {
                case StringOrInt.String s:
                    writer.WriteStringValue(s.Value);
                    break;
                case StringOrInt.Int i:
                    writer.WriteNumberValue(i.Value);
                    break;
                default:
                    throw new ArgumentException($"Unknown StringOrInt: {value}");
            }
        }
    }
}
