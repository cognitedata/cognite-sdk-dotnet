// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The data point class for each individual data point.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The data value.
        /// </summary>
        public MultiValue Value { get; set; }

        /// <summary>
        /// The status code of the datapoint.
        /// 
        /// The most common status codes are:
        /// 
        /// Good (0) (Default)
        /// Uncertain (1073741824)
        /// Bad (2147483648)
        /// 
        /// Only one of code and symbol is required. If both are defined, they must match.
        /// </summary>
        public StatusCode Status { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
