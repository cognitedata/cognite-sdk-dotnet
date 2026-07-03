// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels.Core
{
    /// <summary>
    /// Core data model representation of a set of possible states for a state time series.
    /// Referenced from <see cref="CogniteTimeSeriesBase.StateSet"/> when the time series
    /// <see cref="CogniteTimeSeriesBase.Type"/> is <see cref="TimeSeriesType.State"/>.
    /// </summary>
    public class CogniteStateSet : CogniteDescribable
    {
        /// <summary>
        /// The discrete states a state time series referencing this set may take.
        /// </summary>
        public IEnumerable<CogniteState> States { get; set; }
    }

    /// <summary>
    /// A single state within a <see cref="CogniteStateSet"/>. A state may carry a numeric value,
    /// a string value, or both, matching the value(s) reported on state datapoints.
    /// </summary>
    public class CogniteState
    {
        /// <summary>
        /// Numeric value identifying this state, if any.
        /// </summary>
        public long? NumericValue { get; set; }
        /// <summary>
        /// String value identifying this state, if any.
        /// </summary>
        public string StringValue { get; set; }
    }
}
