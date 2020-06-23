using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// The Asset update class.
    /// </summary>
    public class AssetUpdate
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
        public Update<string> Name { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<string> Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<string> Source { get; set; }

        /// <summary>
        /// Change the ID of the object.
        /// </summary>
        public Update<long?> ParentId { get; set; }

        /// <summary>
        /// Change the external ID of the object.
        /// </summary>
        public Update<long?> ParentExternalId { get; set; }

        /// <summary>
        /// Change the Labels of the object
        /// Currently only available for use in playground
        /// </summary>
        public UpdateLabels<IEnumerable<CogniteExternalId>> Labels { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The asset update item type. Contains the update item for an <see cref="AssetUpdate">AssetUpdate</see>.
    /// </summary>
    public class AssetUpdateItem : UpdateItem<AssetUpdate>
    {
        /// <summary>
        /// Initialize the asset update item with an external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public AssetUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize the asset update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public AssetUpdateItem(long id) : base(id)
        {
        }
    }
}