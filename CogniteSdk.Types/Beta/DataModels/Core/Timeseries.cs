// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Representation of a CDF timeseries in core data models.
    /// </summary>
    public class TimeSeriesBase : CoreInstanceBase
    {
        /// <summary>
        /// Defines whether the time series is a step series or not.
        /// </summary>
        public bool? IsStep { get; set; }
        /// <summary>
        /// Defines whether the time series contains string values or numeric values.
        /// Not updatable - this field cannot be changed after the time series has been
        /// created.
        /// </summary>
        public bool? IsString { get; set; }
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
        /// List of equipment associated with this time series.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Equipment { get; set; }
    }
}