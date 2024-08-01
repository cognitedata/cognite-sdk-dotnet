// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
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
                case JsonTokenType.StartArray:
                    var listStr = new List<string>();
                    var listDbl = new List<double>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.Number:
                                listDbl.Add(reader.GetDouble());
                                break;
                            case JsonTokenType.String:
                                listStr.Add(reader.GetString());
                                break;
                            default:
                                throw new JsonException($"Unable to parse value of type: {reader.TokenType}");
                        }
                    }
                    if (listStr.Count > 0 && listDbl.Count > 0)
                    {
                        throw new JsonException("Unable to parse value of type: mixed array");
                    } else if (listStr.Count > 0)
                    {
                        return SimulatorValue.Create(listStr);
                    }
                    return SimulatorValue.Create(listDbl);
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
                case SimulatorValue.DoubleArray doubleArr:
                    writer.WriteStartArray();
                    foreach (var val in doubleArr.Value)
                    {
                        writer.WriteNumberValue(val);

                    }
                    writer.WriteEndArray();
                    break;
                case SimulatorValue.StringArray stringArr:
                    writer.WriteStartArray();
                    foreach (var val in stringArr.Value)
                    {
                        writer.WriteStringValue(val);
                    }
                    writer.WriteEndArray();
                    break;
                default:
                    throw new ArgumentException($"Unknown SimulatorValue: {value}");
            }
        }
    }

    /// <summary>
    /// Creates SimulatorRoutineRevisionDataSampling values from the JSON input.
    /// </summary>
    public class SimulatorRoutineRevisionDataSamplingConverter : JsonConverter<ISimulatorRoutineRevisionDataSampling>
    {
        /// <summary>
        /// Reads JSON and converts it to an instance of ISimulatorRoutineRevisionDataSampling.
        /// </summary>
        public override ISimulatorRoutineRevisionDataSampling Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;

                // Check if "enabled" property is present and get its value
                if (root.TryGetProperty("enabled", out JsonElement enabledProperty) && enabledProperty.GetBoolean())
                {
                    // Deserialize into SimulatorRoutineRevisionDataSamplingEnabled
                    return JsonSerializer.Deserialize<SimulatorRoutineRevisionDataSamplingEnabled>(root.GetRawText(), options);
                }
                else
                {
                    // Default to SimulatorRoutineRevisionDataSamplingDisabled
                    return new SimulatorRoutineRevisionDataSamplingDisabled();
                }
            }
        }

        /// <summary>
        /// Writes SimulatorRoutineRevisionDataSampling values to JSON.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, ISimulatorRoutineRevisionDataSampling value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

}

