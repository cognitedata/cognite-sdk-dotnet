using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types
{
    public class Asset
    {
        /// External Id provided by client. Must be unique within the project.
        public string ExternalId { get; set; }
        /// The name of the asset.
        public string Name { get; set; }
        /// The parent ID of the asset.
        public long ParentId { get; set; }
        /// The description of the asset.
        public string Description { get; set; }
        /// Custom, application specific metadata. String key -> String value
        public IDictionary<string, string> MetaData { get; set; }
        /// The source of this asset
        public string Source { get; set; }
        /// The Id of the asset.
        public long Id { get; set; }
        /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
        public long CreatedTime { get; set; }
        /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
        public long LastUpdatedTime { get; set; }
        /// InternalId of the root object
        public long RootId { get; set; }
        /// External Id of parent asset provided by client. Must be unique within the project.
        public string ParentExternalId { get; set; }
    }
}