// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Converts multiple values values from JSON to DoubleValue or StringValue.
    /// </summary>
    public class SimulatorValueConverter : JsonConverter<SimulatorValue>
    {
        /// <summary>
        /// Creates SimulatorValue values from the JSON input.
        /// </summary>
        /// <returns>The SimulatorValue. Either DoubleValue or StringValue.</returns>
        public override SimulatorValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return SimulatorValue.Create(reader.GetString());
                case JsonTokenType.Number:
                    return SimulatorValue.Create(reader.GetDouble());
                default:
                    throw new JsonException($"Unable to parse value of type: {reader.TokenType}");
            }
        }

        /// <summary>
        /// Writes SimulatorValue values to JSON numbers or strings.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, SimulatorValue value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            switch (value)
            {
                case SimulatorValue.String s:
                    writer.WriteStringValue(s.Value);
                    break;
                case SimulatorValue.Double d:
                    writer.WriteNumberValue(d.Value);
                    break;
                default:
                    throw new ArgumentException($"Unknown SimulatorValue: {value}");
            }
        }
    }

}
