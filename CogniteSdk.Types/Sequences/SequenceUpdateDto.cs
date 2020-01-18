using System.Collections.Generic;
using CogniteSdk;

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
        public UpdateType<string> Name { get; set; }

        /// <summary>
        /// Set a new value for the Description, or remove the value.
        /// </summary>
        public UpdateType<string> Description { get; set; }

        /// <summary>
        /// Set a new value for the assetId, or remove the value.
        /// </summary>
        public UpdateType<long> AssetId { get; set; }

        /// <summary>
        /// Set a new value for the ExternalId, or remove the value.
        /// </summary>
        public UpdateType<string> ExternalId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateType<IDictionary<string, string>> Metadata { get; set; }
    }
}