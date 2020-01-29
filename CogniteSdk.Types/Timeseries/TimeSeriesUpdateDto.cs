using CogniteSdk.Types.Common;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The time series update DTO.
    /// </summary>
    public class TimeSeriesUpdateDto : Stringable
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
        public DictUpdate<string> Metadata { get; set; }

        /// <summary>
        /// The change that will be applied to the key.
        /// </summary>
        public Update<string> Unit { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public Update<long?> AssetId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Description { get; set; }

        /// <summary>
        /// Change that will be applied to the array.
        /// </summary>
        public SequenceUpdate<long?> SecurityCategories { get; set; }
    }

    /// <summary>
    /// The time series update item DTO. Contains the update item for an <see cref="TimeSeriesUpdateDto">TimeSeriesUpdateDto</see>.
    /// </summary>
    public class TimeSeriesUpdateItem : UpdateItem<TimeSeriesUpdateDto> { }
}