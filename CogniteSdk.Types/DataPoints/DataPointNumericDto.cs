using System.Text.Json.Serialization;

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// The numeric data point DTO.
    /// </summary>
    public abstract class DataPointType
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Timestamp { get; set; }
    }
    
    public class DataPointNumericDto : DataPointType
    {
        /// <summary>
        /// The data value.
        /// </summary>
        public double Value { get; set; }
    }
    
    public class DataPointStringDto : DataPointType
    {
        /// <summary>
        /// The data value.
        /// </summary>
        public string Value { get; set; }
    }
}