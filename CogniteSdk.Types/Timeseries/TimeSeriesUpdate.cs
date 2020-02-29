using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The time series update DTO.
    /// </summary>
    public class TimeSeriesUpdate
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<long?> DataSetId { get; set; }

        /// <summary>
        /// Set a new value for the string.
        /// </summary>
        public UpdateNullable<string> Name { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }

        /// <summary>
        /// The change that will be applied to the key.
        /// </summary>
        public UpdateNullable<string> Unit { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public UpdateNullable<long?> AssetId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<string> Description { get; set; }

        /// <summary>
        /// Change that will be applied to the array.
        /// </summary>
        public UpdateEnumerable<long?> SecurityCategories { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The time series update item DTO. Contains the update item for an <see cref="TimeSeriesUpdate">TimeSeriesUpdate</see>.
    /// </summary>
    public class TimeSeriesUpdateItem : UpdateItem<TimeSeriesUpdate>
    {
        /// <summary>
        /// Initialize the time series update item with an external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public TimeSeriesUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize the time series update item with an internal Id.
        /// </summary>
        /// <param name="internalId">Internal Id to set.</param>
        public TimeSeriesUpdateItem(long internalId) : base(internalId)
        {
        }
    }
}