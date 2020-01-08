using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types
{
    public class AssetWriteDto
    {
        /// <summary>
        /// External Id provided by client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parent ID of the asset.
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// The source of this asset
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// External Id of parent asset provided by client. Must be unique within the project.
        /// </summary>
        public string ParentExternalId { get; set; }
    }
}
