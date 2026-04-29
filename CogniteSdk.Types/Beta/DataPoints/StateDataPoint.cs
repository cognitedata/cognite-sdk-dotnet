// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// A single state-valued data point (numeric and/or string state value).
    /// State time series are currently in private beta.
    /// </summary>
    public class StateDataPoint
    {
        /// <summary>
        /// Timestamp in milliseconds since epoch.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Numeric state value, if present.
        /// </summary>
        public long? NumericValue { get; set; }

        /// <summary>
        /// String state value, if present.
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// Status for this data point.
        /// </summary>
        public StatusCode Status { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
