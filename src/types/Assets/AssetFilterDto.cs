using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Types.Assets
{
    public class AssetFilterDto
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Return only the direct descendants of the specified assets.
        /// </summary>
        public IEnumerable<long> ParentIds { get; set; }

        /// <summary>
        /// Return only the direct descendants of the specified assets.
        /// </summary>
        public IEnumerable<string> ParentExternalIds { get; set; }

        /// <summary>
        /// Only include these root assets and their descendants.
        /// </summary>
        public IEnumerable<Identity> RootIds { get; set; }

        /// <summary>
        /// Only include assets in subtrees rooted at the specified assets (including the roots given). If the total
        /// size of the given subtrees exceeds 100,000 assets, an error will be returned.
        /// </summary>
        public IEnumerable<Identity> AssetSubtreeIds { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// The source of the asset.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }

        /// <summary>
        /// Whether the filtered assets are root assets, or not. Set to True to only list root assets.
        /// </summary>
        public bool Root { get; set; }

        /// <summary>
        /// Filter by this (case-sensitive) prefix for the external ID.
        /// </summary>
        public string ExternalIdPrefix { get; set; }
    }
}