// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// TimeSeries read class (without metadata).
    /// </summary>
    public class TimeSeriesWithoutMetadata
    {
        /// <summary>
        /// Server-generated ID for the object
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The externally supplied ID for the time series
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Javascript friendly internal ID given to the object.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// The display short name of the time series.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether the time series is string valued or not.
        /// </summary>
        public bool IsString { get; set; }

        /// <summary>
        /// The physical unit of the time series.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// AssetId for the asset this time series is linked to.
        /// </summary>
        public long? AssetId { get; set; }

        /// <summary>
        /// Whether the time series is a step series or not.
        /// </summary>
        public bool IsStep { get; set; }

        /// <summary>
        /// Description of the time series.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The required security categories to access this time series.
        /// </summary>
        public IEnumerable<long> SecurityCategories { get; set; }

        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// TimeSeries read class.
    /// </summary>
    public class TimeSeries : TimeSeriesWithoutMetadata
    {
        /// <summary>
        /// Custom, application specific metadata. Maximum length of key is 32 bytes, value 512 bytes, up to 16
        /// key-value pairs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }
    }
}
