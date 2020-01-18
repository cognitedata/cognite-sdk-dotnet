using System.Collections.Generic;
using CogniteSdk;

namespace CogniteSdk.Events
{
    /// <summary>
    /// The event update DTO.
    /// </summary>
    public class EventUpdateDto
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the start time, or remove the value.
        /// </summary>
        public UpdateType<long> StartTime { get; set; }

        /// <summary>
        /// Set a new value for the end time, or remove the value.
        /// </summary>
        public UpdateType<long> EndTime { get; set; }

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
        /// Change that will be applied to the array.
        /// </summary>
        public UpdateType<IEnumerable<long>> AssetIds { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> Source { get; set; }

        /// <summary>
        /// Change the external ID of the object.
        /// </summary>
        public UpdateType<long> ParentExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> Type { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> SubType { get; set; }
    }
}