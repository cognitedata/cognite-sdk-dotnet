using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Converts ErrorValue values from JSON to LongValue, DoubleValue or StringValue.
    /// </summary>
    public class ValueTypeConverter : JsonConverter<ValueType>
    {
        /// <summary>
        /// Produces error values from the JSON input.
        /// </summary>
        /// <returns>The ErrorValue. Either LongValue, DoubleValue or StringValue.</returns>
        public override ValueType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return new StringValue { Value=reader.GetString() };
                case JsonTokenType.Number:
                    var value = reader.GetString();
                    if (Int64.TryParse(value, out long longNumber))
                    {
                        return new LongValue { Value=longNumber };
                    }

                    if (Double.TryParse(reader.GetString(), out double doubleNumber))
                    {
                        return new DoubleValue { Value=doubleNumber };
                    }

                    throw new JsonException($"Unable to parse number value: {value}");
                default:
                    throw new JsonException($"Unable to parse value of type: {reader.TokenType}");
            }
        }

        /// <summary>
        /// Not in use. We don't write any error values.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, ValueType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}