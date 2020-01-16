using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Converts ErrorValue values from JSON to LongValue, DoubleValue or StringValue.
    /// </summary>
    public class ErrorConverter : JsonConverter<ErrorValue>
    {
        /// <summary>
        /// Produces error values from the JSON input.
        /// </summary>
        /// <returns>ErrorValue value.</returns>
        public override ErrorValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return new StringValue { String=reader.GetString() };
                case JsonTokenType.Number:
                    if (Int64.TryParse(reader.GetString(), out long longNumber))
                    {
                        return new LongValue { Value=longNumber };
                    }
                    if (Double.TryParse(reader.GetString(), out double doubleNumber))
                    {
                        return new DoubleValue { Value=doubleNumber };
                    }
                    return new StringValue { String="Unable to parse value" };
                default:
                    return new StringValue { String="Unable to parse value" };
            }
        }

        /// <summary>
        /// Not in use. We don't write any error values.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, ErrorValue value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}