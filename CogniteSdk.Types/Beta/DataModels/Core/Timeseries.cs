// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Representation of a CDF timeseries in core data models.
    /// </summary>
    public class CogniteTimeSeriesBase : CogniteCoreInstanceBase
    {
        /// <summary>
        /// Defines whether the time series is a step series or not.
        /// </summary>
        public bool? IsStep { get; set; }
        /// <summary>
        /// Type of datapoints the time series contains.
        /// </summary>
        public TimeSeriesType Type { get; set; }

        /// <summary>
        /// The physical unit of the time series as described in the source.
        /// </summary>
        public string SourceUnit { get; set; }
        /// <summary>
        /// Direct relation to unit in the `cdf_units` space.
        /// </summary>
        /// <value></value>
        public DirectRelationIdentifier Unit { get; set; }
        /// <summary>
        /// List of assets associated with this time series.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Assets { get; set; }
        /// <summary>
        /// List of activities associated with this time series.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Activities { get; set; }
        /// <summary>
        /// List of equipment associated with this time series.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Equipment { get; set; }
    }

    /// <summary>
    /// Type of datapoints the time series contains.
    /// </summary>
    public enum TimeSeriesType
    {
        /// <summary>
        /// Time series containing string values.
        /// </summary>
        String,
        /// <summary>
        /// Time series containing numeric values.
        /// </summary>
        Numeric,
    }
}