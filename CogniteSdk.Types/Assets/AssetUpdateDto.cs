using System.Collections.Generic;

namespace CogniteSdk.Assets
{
    /// <summary>
    /// The Asset update DTO.
    /// </summary>
    public class AssetUpdateDto
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string.
        /// </summary>
        public UpdateType<string> Name { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateType<IDictionary<string, string>> Metadata { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> Source { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public UpdateType<long> ParentId { get; set; }

        /// <summary>
        /// Change the external ID of the object.
        /// </summary>
        public UpdateType<long> ParentExternalId { get; set; }
    }
}