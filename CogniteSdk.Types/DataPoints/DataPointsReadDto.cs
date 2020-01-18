using System.Text.Json.Serialization;

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// Data Points DTO.
    /// </summary>
    public class DataPointsReadDto
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The external ID of the time series the datapoints belong to.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Whether the time series is string valued or not.
        /// </summary>
        /// <value></value>
        public bool IsString { get; set; }

        /// <summary>
        /// Whether the time series is a step series or not.
        /// </summary>
        public bool? IsStep { get; set; }

        /// <summary>
        /// The physical unit of the time series.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The list of datapoints.
        /// </summary>
        [JsonPropertyName("datapoints")]
        public DataPointNumericDto DataPoints { get; set; }
    }
}