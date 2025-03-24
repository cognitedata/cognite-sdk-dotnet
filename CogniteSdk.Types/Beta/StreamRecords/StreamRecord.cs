// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.DataModels;

namespace CogniteSdk.Beta
{
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
        /// </summary>
        public T Properties { get; set; }
    }
}
