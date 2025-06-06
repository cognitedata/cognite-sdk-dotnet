using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The event update class.
    /// </summary>
    public class EventUpdate
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<long?> DataSetId { get; set; }

        /// <summary>
        /// Set a new value for the start time, or remove the value.
        /// </summary>
        public Update<long?> StartTime { get; set; }

        /// <summary>
        /// Set a new value for the end time, or remove the value.
        /// </summary>
        public Update<long?> EndTime { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }

        /// <summary>
        /// Change that will be applied to the array.
        /// </summary>
        public UpdateEnumerable<long?> AssetIds { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Source { get; set; }

        /// <summary>
        /// Change the external ID of the object.
        /// </summary>
        public Update<long?> ParentExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Type { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> SubType { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The asset update item DTO. Contains the update item for an <see cref="EventUpdate">EventUpdate</see>.
    /// </summary>
    public class EventUpdateItem : UpdateItem<EventUpdate>
    {
        /// <summary>
        /// Initialize the event update item with an external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public EventUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize the event update item with an internal Id.
        /// </summary>
        /// <param name="internalId">Internal Id to set.</param>
        public EventUpdateItem(long internalId) : base(internalId)
        {
        }
    }
}
