// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// Data Points write type
    /// </summary>
    public class DataPointsWriteDto
    {
        /// <summary>
        /// The list of data points. The limit per request is 100000 data points.
        /// </summary>
        public IEnumerable<DataPointDto> DataPoints { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public Identity Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public Identity ExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsWriteDto>(this);
    }
}