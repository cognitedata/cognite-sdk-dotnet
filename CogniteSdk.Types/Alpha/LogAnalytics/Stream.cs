namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Request to create an ILA stream.
    /// </summary>
    public class StreamWrite
    {
        /// <summary>
        /// Stream external ID. Must be unique within the project and a valid stream identifier.
        /// Stream identifiers can only consist of alphanumeric characters, hyphens, and underscores.
        /// It must not start with cdf_ or cognite_, as those are reserved for future use.
        /// Stream id cannot be logs or records. Max length is 100 characters.
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// An ILA stream.
    /// </summary>
    public class Stream : StreamWrite
    {
        /// <summary>
        /// Time the stream was created, in milliseconds since epoch.
        /// </summary>
        public int CreatedTime { get; set; }
    }
}