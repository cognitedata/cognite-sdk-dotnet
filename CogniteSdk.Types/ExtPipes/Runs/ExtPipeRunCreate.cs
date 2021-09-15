using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Status of an extraction pipeline run
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExtPipeRunStatus
    {
        /// <summary>
        /// Successful run
        /// </summary>
        success,
        /// <summary>
        /// Failed run
        /// </summary>
        failure,
        /// <summary>
        /// Run heartbeat
        /// </summary>
        seen
    }

    /// <summary>
    /// Create an extraction pipeline run update.
    /// </summary>
    public class ExtPipeRunCreate
    {
        /// <summary>
        /// Extraction pipeline externalId.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Run status
        /// </summary>
        public ExtPipeRunStatus Status { get; set; }
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Created time, in milliseconds since 00:00:00 Thursday 1 January 1970, minus leap seconds.
        /// Leave as null to use current time.
        /// </summary>
        public long? CreatedTime { get; set; }
    }
}
