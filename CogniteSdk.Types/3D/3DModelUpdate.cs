using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDModel update class.
    /// </summary>
    public class ThreeDModelUpdate
    {
        /// <summary>
        /// Set a new value for the string.
        /// </summary>
        public Update<string> Name { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The ThreeDModel update item type. Contains the update item for an <see cref="ThreeDModelUpdate">ThreeDModelUpdate</see>.
    /// </summary>
    public class ThreeDModelUpdateItem : UpdateItem<ThreeDModelUpdate>
    {
        /// <summary>
        /// Initialize the ThreeDModel update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public ThreeDModelUpdateItem(long id) : base(id)
        {
        }
    }
}