using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                default:
                    throw new ArgumentException($"Unknown MultiValue: {value}");
            }
        }
    }
}