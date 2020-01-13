using System.Collections.Generic;

using CogniteSdk;

namespace CogniteSdk.Assets
{
    public class AssetDeleteDto
    {
        /// <summary>
        /// Sequence of IdentityId or IdentityExternalId (required).
        /// </summary>
        /// <value></value>
        public IEnumerable<Identity> Items { get; set; }

        /// <summary>
        /// Recursively delete all asset subtrees under the specified IDs. Default: false
        /// </summary>
        public bool? Recursive { get; set; }

        /// <summary>
        /// Ignore IDs and external IDs that are not found. Default: false
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }
    }
}