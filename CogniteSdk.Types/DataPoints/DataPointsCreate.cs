// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Data Points write class.
    /// </summary>
    public class DataPointsCreate
    {
        /// <summary>
        /// The list of data points. The limit per request is 100000 data points.
        /// </summary>
        public IEnumerable<DataPoint> DataPoints { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public Identity Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public Identity ExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsCreate>(this);
    }
}