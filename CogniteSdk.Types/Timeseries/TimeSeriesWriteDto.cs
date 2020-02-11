// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// Timeseries write dto.
    /// </summary>
    public class TimeSeriesWriteDto
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
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The physical unit of the time series.
        /// </summary>
        public string Unit { get; set; }

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
