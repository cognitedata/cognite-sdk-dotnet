// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Sequences
{
    public class SequenceFilterDto
    {
        /// <summary>
        /// Return only sequences with this exact name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Filter by this (case-sensitive) prefix for the external ID.
        /// </summary>
        public string ExternalIdPrefix { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Asset IDs of related equipment that this event relates to.
        /// </summary>
        public IEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// Only include events that have a related asset in a tree rooted at any of these root assetIds.
        /// </summary>
        public IEnumerable<Identity> RootAssetIds { get; set; }

        /// <summary>
        /// Only include assets in subtrees rooted at the specified assets (including the roots given). If the total
        /// size of the given subtrees exceeds 100,000 assets, an error will be returned.
        /// </summary>
        public IEnumerable<Identity> AssetSubtreeIds { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }
    }
}
