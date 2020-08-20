// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// The data point class for each individual synthetic data point.
    /// </summary>
    public class DataPointSynthetic
    {
        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The data value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Human readable string with description of what went wrong.
        /// </summary>
        public string Error { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Synthetic data points item class.
    /// </summary>
    public class DataPointsSyntheticItem
    {

        /// <summary>
        /// Whether the returned data points are of string type or floating point type.
        /// Currently it will always be false.
        /// </summary>
        public bool IsString { get; set; }


        /// <summary>
        /// The list of data points.
        /// </summary>
        [JsonPropertyName("datapoints")]
        public IEnumerable<DataPointSynthetic> DataPoints { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
