// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The event delete class.
    /// </summary>
    public class AssetDelete : ItemsWithoutCursor<Identity>
    {
        /// <summary>
        /// Recursively delete all asset subtrees under the specified IDs. Default: false
        /// </summary>
        public bool? Recursive { get; set; }

        /// <summary>
        /// Ignore IDs and external IDs that are not found. Default: false
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
