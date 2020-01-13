using System.Collections.Generic;
using CogniteSdk;

namespace CogniteSdk.TimeSeries
{
    public class TimeSeriesUpdateDto
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
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public Update<IDictionary<string, string>> Metadata { get; set; }

        /// <summary>
        /// The change that will be applied to the key.
        /// </summary>
        public Update<string> Unit { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public Update<long> AssetId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Description { get; set; }

        /// <summary>
        /// Change that will be applied to the array.
        /// </summary>
        public Update<IEnumerable<long>> SecurityCategories { get; set; }
    }
}