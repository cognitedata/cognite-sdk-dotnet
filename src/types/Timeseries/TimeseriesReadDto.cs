using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Timeseries
{
    /// <summary>
    /// Timeseries read dto.
    /// </summary>
    public class TimeseriesReadDto
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
        /// The display short name of the time series.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether the time series is string valued or not.
        /// </summary>
        public bool IsString { get; set; }

        /// <summary>
        /// Custom, application specific metadata.
        /// Maximum length of key is 32 bytes,
        /// value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The physical unit of the time series.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// AssetId for the asset this timeseries is linked to.
        /// </summary>
        public long AssetId { get; set; }

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
        /// Unix UTC timestamp when the timeseries was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Unix UTC timestamp when the timeseries was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
