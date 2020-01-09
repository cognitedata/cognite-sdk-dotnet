using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Types.Assets
{
    public class AssetFilter
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
    }
}