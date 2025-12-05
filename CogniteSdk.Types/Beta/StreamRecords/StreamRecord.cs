// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using CogniteSdk.DataModels;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Status of a sync record, indicating whether it was created, updated, or deleted.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StreamRecordStatus
    {
        /// <summary>
        /// Record was created.
        /// </summary>
        created,

        /// <summary>
        /// Record was updated.
        /// </summary>
        updated,

        /// <summary>
        /// Record was deleted (tombstone record).
        /// Deleted records only have top-level fields and no properties.
        /// </summary>
        deleted
    }

    /// <summary>
    /// A retrieved stream record.
    /// </summary>
    /// <typeparam name="T">Type of the properties bag.</typeparam>
    public class StreamRecord<T>
    {
        /// <summary>
        /// External ID of the record, required.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Id of the space the record belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Time the record was created.
        /// The number of milliseconds since 00:00:00 Thursday 1 January 1970, Coordinated
        /// Unversal Time (UTC) minus leap seconds.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Time the record was last updated.
        /// The number of milliseconds since 00:00:00 Thursday 1 January 1970, Coordinated
        /// Unversal Time (UTC) minus leap seconds.
        /// </summary>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// Spaces to containers to properties and their values for the requested containers.
        /// You can use <see cref="StandardInstanceData"/> as a fallback for generic results here.
        /// Note: This property is omitted for deleted records in mutable streams (tombstones).
        /// </summary>
        public T Properties { get; set; }
        /// <summary>
        /// Status of the record. Only present in sync responses.
        /// Indicates whether the record was created, updated, or deleted.
        /// For deleted records (tombstones), the Properties field will be null.
        /// </summary>
        public StreamRecordStatus? Status { get; set; }
    }
}
