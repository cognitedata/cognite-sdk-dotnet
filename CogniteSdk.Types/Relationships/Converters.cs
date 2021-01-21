// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Converts multiple values values from JSON to LongValue, DoubleValue or StringValue.
    /// </summary>
    public class RelationshipVertexConverter : JsonConverter<RelationshipVertexType>
    {
        /// <summary>
        /// Creates RelationshipVertexType values from the JSON input.
        /// </summary>
        /// <returns>The RelationshipVertexType. Either LongValue, DoubleValue or StringValue.</returns>
        public override RelationshipVertexType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var token = reader.GetString();
                    switch (token)
                    {
                        case "asset":
                            return RelationshipVertexType.Asset;
                        case "timeSeries":
                            return RelationshipVertexType.TimeSeries;
                        case "file":
                            return RelationshipVertexType.File;
                        case "event":
                            return RelationshipVertexType.Event;
                        case "sequence":
                            return RelationshipVertexType.Sequence;
                        default:
                            throw new System.ArgumentException($"Unknown RelationshipVertex Type (Source/Target): {token}");
                    }
                default:
                    throw new JsonException($"Unable to parse value of type: {reader.TokenType}");
            }
        }

        /// <summary>
        /// Writes RelationshipVertexType values to JSON numbers or strings.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, RelationshipVertexType value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            switch (value)
            {
                case RelationshipVertexType.Asset:
                    writer.WriteStringValue("asset");
                    break;
                case RelationshipVertexType.TimeSeries:
                    writer.WriteStringValue("timeSeries");
                    break;
                case RelationshipVertexType.File:
                    writer.WriteStringValue("file");
                    break;
                case RelationshipVertexType.Event:
                    writer.WriteStringValue("event");
                    break;
                case RelationshipVertexType.Sequence:
                    writer.WriteStringValue("sequence");
                    break;
                default:
                    throw new ArgumentException($"Unknown RelationshipVertexType: {value}");
            }
        }
    }
}
