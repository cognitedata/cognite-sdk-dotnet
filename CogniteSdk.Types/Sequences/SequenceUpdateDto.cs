using System.Collections.Generic;


namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The sequence update DTO.
    /// </summary>
    public class SequenceUpdateDto
    {
        /// <summary>
        /// Set a new value for the Name, or remove the value.
        /// </summary>
        public Update<string> Name { get; set; }

        /// <summary>
        /// Set a new value for the Description, or remove the value.
        /// </summary>
        public Update<string> Description { get; set; }

        /// <summary>
        /// Set a new value for the assetId, or remove the value.
        /// </summary>
        public Update<long?> AssetId { get; set; }

        /// <summary>
        /// Set a new value for the ExternalId, or remove the value.
        /// </summary>
        public Update<string> ExternalId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public DictUpdate<string> Metadata { get; set; }
    }
}