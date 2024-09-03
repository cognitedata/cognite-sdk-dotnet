// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Alpha;
using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.Alpha
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
        public IdentityWithInstanceId Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public IdentityWithInstanceId ExternalId { get; set; }

        /// <summary>
        /// The instance ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public IdentityWithInstanceId InstanceId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsCreate>(this);
    }
}