// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Per-state aggregate statistics returned inside an aggregate datapoint
    /// for state time series. State time series are currently in private beta.
    /// </summary>
    public class StateAggregate
    {
        /// <summary>
        /// Numeric state value this aggregate row refers to.
        /// </summary>
        public long NumericValue { get; set; }

        /// <summary>
        /// String state value, if present.
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// Number of occurrences of this state in the aggregate interval.
        /// </summary>
        public long? StateCount { get; set; }

        /// <summary>
        /// Number of transitions into this state in the aggregate interval.
        /// </summary>
        public long? StateTransitions { get; set; }

        /// <summary>
        /// Total duration of this state in the aggregate interval (milliseconds).
        /// </summary>
        public long? StateDuration { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
