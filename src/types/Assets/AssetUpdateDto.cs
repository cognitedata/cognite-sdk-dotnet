using System.Collections.Generic;
using CogniteSdk;

namespace CogniteSdk.Assets
{
    public class AssetUpdateDto
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string.
        /// </summary>
        public Update<string> Name { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public Update<IDictionary<string, string>> MetaData { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Source { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public Update<long> ParentId { get; set; }

        /// <summary>
        /// Change the external ID of the object.
        /// </summary>
        public Update<long> ParentExternalId { get; set; }
    }
}