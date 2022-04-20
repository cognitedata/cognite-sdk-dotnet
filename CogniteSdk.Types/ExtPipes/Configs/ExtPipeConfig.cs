namespace CogniteSdk
{
    /// <summary>
    /// Extraction pipeline configuration retrieved from CDF.
    /// </summary>
    public class ExtPipeConfig
    {
        /// <summary>
        /// ExternalId of the extraction pipeline this configuration belongs to.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Raw configuration text.
        /// </summary>
        public string Config { get; set; }
        /// <summary>
        /// Revision of this configuration, starts at 1.
        /// </summary>
        public int Revision { get; set; }
        /// <summary>
        /// Creation timestamp in ms since 01/01/1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Description of this revision.
        /// </summary>
        public string Description { get; set; }
    }
}
