// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// The data point DTO for each individual data point.
    /// </summary>
    public class DataPointDto : Stringable
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The data value.
        /// </summary>
        public MultiValue Value { get; set; }
    }
}