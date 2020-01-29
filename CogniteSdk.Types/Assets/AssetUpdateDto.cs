using CogniteSdk.Types.Common;

namespace CogniteSdk.Assets
{
    /// <summary>
    /// The Asset update DTO.
    /// </summary>
    public class AssetUpdateDto : Stringable
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string.
        /// </summary>
        public SetUpdate<string> Name { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public DictUpdate<string> Metadata { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Source { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public SetUpdate<long?> ParentId { get; set; }

        /// <summary>
        /// Change the external ID of the object.
        /// </summary>
        public SetUpdate<long?> ParentExternalId { get; set; }
    }

    /// <summary>
    /// The asset update item DTO. Contains the update item for an <see cref="AssetUpdateDto">AssetUpdateDto</see>.
    /// </summary>
    public class AssetUpdateItem : UpdateItem<AssetUpdateDto> { }
}