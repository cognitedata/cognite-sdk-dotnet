// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Converts StreamTemplateName enum to and from JSON strings.
    /// </summary>
    public class StreamTemplateNameConverter : JsonConverter<StreamTemplateName>
    {
        /// <summary>
        /// Reads a JSON string and converts it to StreamTemplateName.
        /// </summary>
        public override StreamTemplateName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value switch
            {
                "ImmutableTestStream" => StreamTemplateName.ImmutableTestStream,
                "ImmutableDataStaging" => StreamTemplateName.ImmutableDataStaging,
                "ImmutableNormalizedData" => StreamTemplateName.ImmutableNormalizedData,
                "ImmutableArchive" => StreamTemplateName.ImmutableArchive,
                "MutableTestStream" => StreamTemplateName.MutableTestStream,
                "MutableLiveData" => StreamTemplateName.MutableLiveData,
                _ => throw new JsonException($"Unknown stream template name: {value}")
            };
        }

        /// <summary>
        /// Writes a StreamTemplateName value as a JSON string.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, StreamTemplateName value, JsonSerializerOptions options)
        {
            var stringValue = value switch
            {
                StreamTemplateName.ImmutableTestStream => "ImmutableTestStream",
                StreamTemplateName.ImmutableDataStaging => "ImmutableDataStaging",
                StreamTemplateName.ImmutableNormalizedData => "ImmutableNormalizedData",
                StreamTemplateName.ImmutableArchive => "ImmutableArchive",
                StreamTemplateName.MutableTestStream => "MutableTestStream",
                StreamTemplateName.MutableLiveData => "MutableLiveData",
                _ => throw new JsonException($"Unknown stream template name value: {value}")
            };
            writer.WriteStringValue(stringValue);
        }
    }
}
