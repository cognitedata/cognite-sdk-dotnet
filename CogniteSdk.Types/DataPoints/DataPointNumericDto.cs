using System.Text.Json.Serialization;

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// The numeric data point DTO.
    /// </summary>
    public class DataPointNumericDto
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The data value.
        /// </summary>
        public double Value { get; set; }
    }
}