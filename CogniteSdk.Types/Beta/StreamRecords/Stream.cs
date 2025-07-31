// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request to create a stream.
    /// </summary>
    public class StreamWrite
    {
        /// <summary>
        /// Stream external ID. Must be unique within the project and a valid stream identifier.
        /// Stream identifiers can only consist of alphanumeric characters, hyphens, and underscores.
        /// It must not start with cdf_ or cognite_, as those are reserved for future use.
        /// Stream id cannot be "logs" or "records". Max length is 100 characters.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Settings for the stream. Required field.
        /// </summary>
        [JsonPropertyName("settings")]
        public StreamSettings Settings { get; set; }
    }

    /// <summary>
    /// Settings for a stream.
    /// </summary>
    public class StreamSettings
    {
        /// <summary>
        /// Template settings for the stream.
        /// </summary>
        [JsonPropertyName("template")]
        public StreamTemplateSettings Template { get; set; }
    }

    /// <summary>
    /// The name of the stream template.
    /// </summary>
    [JsonConverter(typeof(StreamTemplateNameConverter))]
    public enum StreamTemplateName
    {
        /// <summary>
        /// Immutable test stream template
        /// </summary>
        ImmutableTestStream,

        /// <summary>
        /// Immutable data staging template
        /// </summary>
        ImmutableDataStaging,

        /// <summary>
        /// Immutable normalized data template
        /// </summary>
        ImmutableNormalizedData,

        /// <summary>
        /// Immutable archive template
        /// </summary>
        ImmutableArchive,

        /// <summary>
        /// Mutable test stream template
        /// </summary>
        MutableTestStream,

        /// <summary>
        /// Mutable live data template
        /// </summary>
        MutableLiveData
    }

    /// <summary>
    /// Template settings for a stream.
    /// </summary>
    public class StreamTemplateSettings
    {
        /// <summary>
        /// The name of the stream template.
        /// </summary>
        [JsonPropertyName("name")]
        public StreamTemplateName Name { get; set; }
    }

    /// <summary>
    /// A stream.
    /// </summary>
    public class Stream : StreamWrite
    {
        /// <summary>
        /// Time the stream was created, in milliseconds since epoch.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Name of the template used for creating this stream.
        /// Only present when using alpha cdf-version header.
        /// </summary>
        [JsonPropertyName("createdFromTemplate")]
        public string CreatedFromTemplate { get; set; }

        /// <summary>
        /// Type of the stream (Immutable or Mutable).
        /// Only present when using alpha cdf-version header.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
