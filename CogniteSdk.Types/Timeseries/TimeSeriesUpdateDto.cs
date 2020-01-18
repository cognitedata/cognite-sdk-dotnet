using System.Collections.Generic;
using CogniteSdk;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The time series update DTO.
    /// </summary>
    public class TimeSeriesUpdateDto
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
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateType<IDictionary<string, string>> Metadata { get; set; }

        /// <summary>
        /// The change that will be applied to the key.
        /// </summary>
        public UpdateType<string> Unit { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public UpdateType<long> AssetId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateType<string> Description { get; set; }

        /// <summary>
        /// Change that will be applied to the array.
        /// </summary>
        public UpdateType<IEnumerable<long>> SecurityCategories { get; set; }
    }
}