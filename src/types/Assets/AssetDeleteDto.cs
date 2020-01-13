// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets
{
    public class AssetDeleteDto : ItemsWithoutCursor<Identity>
    {
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