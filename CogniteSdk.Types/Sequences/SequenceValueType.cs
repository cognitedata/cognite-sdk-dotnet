using System.Text.Json.Serialization;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// Type for datapoints in a column.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SequenceValueType
    {
        /// String value
        STRING,
        /// Double value
        DOUBLE,
        /// Long value
        LONG
    }
}