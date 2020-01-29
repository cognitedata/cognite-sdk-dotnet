// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// Identity class with time range.
    /// </summary>
    public class IdentityWithRange : Stringable
    {
        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        /// </summary>
        public long InclusiveBegin { get; set; }

        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        /// </summary>
        public long? ExclusiveEnd { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// Data Points delete DTO.
    /// </summary>
    public class DataPointsDeleteDto : ItemsWithoutCursor<IdentityWithRange> {}
}