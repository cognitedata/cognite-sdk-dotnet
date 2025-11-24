// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Item identifying a stream to delete.
    /// </summary>
    public class StreamDeleteItem
    {
        /// <summary>
        /// Stream external ID to delete.
        /// </summary>
        public string ExternalId { get; set; }
    }

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
        public StreamTemplateSettings Template { get; set; }
    }

    /// <summary>
    /// Template settings for a stream.
    /// </summary>
    public class StreamTemplateSettings
    {
        /// <summary>
        /// The name of the stream template.
        /// Stream template names are dynamically defined per CDF project.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Type of stream, either Immutable or Mutable.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StreamType
    {
        /// <summary>
        /// Immutable stream type.
        /// </summary>
        Immutable,
        /// <summary>
        /// Mutable stream type.
        /// </summary>
        Mutable
    }

    /// <summary>
    /// A stream limit with provisioned and consumed values.
    /// </summary>
    public class StreamLimit
    {
        /// <summary>
        /// Amount of resource provisioned.
        /// </summary>
        public double Provisioned { get; set; }

        /// <summary>
        /// Amount of resource consumed.
        /// </summary>
        public double? Consumed { get; set; }
    }

    /// <summary>
    /// Stream limit settings containing usage limits and statistics.
    /// </summary>
    public class StreamLimitSettings
    {
        /// <summary>
        /// Maximum number of records that can be stored in the stream.
        /// </summary>
        public StreamLimit MaxRecordsTotal { get; set; }

        /// <summary>
        /// Maximum amount of data that can be stored in the stream, in gigabytes.
        /// </summary>
        public StreamLimit MaxGigaBytesTotal { get; set; }

        /// <summary>
        /// Maximum length of time that the lastUpdatedTime filter can retrieve records for, in ISO-8601 format.
        /// This setting is only available for immutable streams.
        /// </summary>
        public string MaxFilteringInterval { get; set; }
    }

    /// <summary>
    /// Stream lifecycle settings defining data retention policies.
    /// </summary>
    public class StreamLifecycleSettings
    {
        /// <summary>
        /// Time for which records are kept in hot storage after creation, in ISO-8601 format.
        /// This setting is available only for immutable streams.
        /// </summary>
        public string HotPhaseDuration { get; set; }

        /// <summary>
        /// ISO-8601 formatted time specifying how long to retain a record in this stream.
        /// After this time passes, records are scheduled to be removed from the stream.
        /// This setting is available only for immutable streams.
        /// </summary>
        public string DataDeletedAfter { get; set; }

        /// <summary>
        /// Time until the soft deleted stream will actually be deleted by the system, in an ISO-8601 compliant date-time format.
        /// </summary>
        public string RetainedAfterSoftDelete { get; set; }
    }

    /// <summary>
    /// Stream response settings containing lifecycle and limit information.
    /// </summary>
    public class StreamResponseSettings
    {
        /// <summary>
        /// Data lifecycle settings. These settings are populated from the stream creation template.
        /// </summary>
        public StreamLifecycleSettings Lifecycle { get; set; }

        /// <summary>
        /// Limits and usage information.
        /// </summary>
        public StreamLimitSettings Limits { get; set; }
    }

    /// <summary>
    /// A stream.
    /// </summary>
    public class Stream
    {
        /// <summary>
        /// Stream external ID. Must be unique within the project and a valid stream identifier.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Time the stream was created, in milliseconds since epoch.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Name of the template used for creating this stream.
        /// </summary>
        public string CreatedFromTemplate { get; set; }

        /// <summary>
        /// Type of the stream (Immutable or Mutable).
        /// </summary>
        public StreamType Type { get; set; }

        /// <summary>
        /// Stream settings containing lifecycle and limit information.
        /// </summary>
        public StreamResponseSettings Settings { get; set; }
    }
}
