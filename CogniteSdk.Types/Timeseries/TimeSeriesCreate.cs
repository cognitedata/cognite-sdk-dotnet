// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Timeseries write class.
    /// </summary>
    public class TimeSeriesCreate
    {
        /// <summary>
        /// Externally provided ID for the time series (optional, but recommended.)
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Javascript friendly internal ID given to the object.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Human readable name of the time series.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set a value for legacyName to allow applications using
        /// API v0.3, v04, v05, and v0.6 to access this time series.
        /// The legacy name is the human-readable name for the time
        /// series and is mapped to the name field used in API versions
        /// 0.3-0.6. The legacyName field value must be unique, and setting
        /// this value to an already existing value will return an error.
        /// We recommend that you set this field to the same value as externalId.
        /// </summary>
        public string LegacyName { get; set; }

        /// <summary>
        /// Whether the time series is string valued or not.
        /// </summary>
        public bool IsString { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// Limits: Maximum length of key is 32 bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The physical unit of the time series (free-text field).
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// The physical unit of the time series (reference to unit catalog). Only available for numeric time series.
        /// </summary>
        public string UnitExternalId { get; set; }

        /// <summary>
        /// Asset this time series should be related to.
        /// </summary>
        public long? AssetId { get; set; }

        /// <summary>
        /// Whether the time series is a step series or not.
        /// </summary>
        public bool IsStep { get; set; }

        /// <summary>
        /// A description of the time series.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The required security categories to access this time series.
        /// </summary>
        public IEnumerable<long> SecurityCategories { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
