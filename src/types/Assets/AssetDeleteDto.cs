using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk.Types.Assets
{
    public class AssetDeleteDto
    {
        /// <summary>
        /// Sequence of IdentityId or IdentityExternalId.
        /// </summary>
        /// <value></value>
        public IEnumerable<Identity> items { get; set; }

        /// <summary>
        /// Recursively delete all asset subtrees under the specified IDs.
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Ignore IDs and external IDs that are not found.
        /// </summary>
        public bool ignoreUnknownIds { get; set; }
    }
}