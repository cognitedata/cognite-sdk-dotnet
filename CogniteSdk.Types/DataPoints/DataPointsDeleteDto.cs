// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// Data Points abstract delete type. Either DataPointsDeleteById or DataPointsDeleteByExternalId
    /// </summary>
    public abstract class DataPointsDeleteType
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
        public long ExclusiveEnd { get; set; }
    }

    /// <summary>
    /// Data Points abstract delete by Id DTO
    /// </summary>
    public abstract class DataPointsDeleteByIdDto
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public Identity Id { get; set; }
    }

    /// <summary>
    /// Data Points abstract delete by External Id DTO
    /// </summary>
    public abstract class DataPointsDeleteByExternalIdDto
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public Identity ExternalId { get; set; }
    }
}