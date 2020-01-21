using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Converts ErrorValue values from JSON to LongValue, DoubleValue or StringValue.
    /// </summary>
    public class MultiValueConverter : JsonConverter<MultiValue>
    {
        /// <summary>
        /// Produces error values from the JSON input.
        /// </summary>
        /// <returns>The ErrorValue. Either LongValue, DoubleValue or StringValue.</returns>
        public override MultiValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return MultiValue.Create(reader.GetString());
                case JsonTokenType.Number:
                    var value = reader.GetString();
                    if (Int64.TryParse(value, out long longNumber))
                    {
                        return MultiValue.Create(longNumber);
                    }

                    if (Double.TryParse(reader.GetString(), out double doubleNumber))
                    {
                        return MultiValue.Create(doubleNumber);
                    }

                    throw new JsonException($"Unable to parse number value: {value}");
                default:
                    throw new JsonException($"Unable to parse value of type: {reader.TokenType}");
            }
        }

        /// <summary>
        /// Not in use. We don't write any error values.
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
                    writer.WriteStringValue(d.ToString());
                    break;
                case MultiValue.Long l:
                    writer.WriteStringValue(l.ToString());
                    break;
                default:
                    throw new ArgumentException($"Unknown MultiValue: {value}");
            }
        }
    }
}