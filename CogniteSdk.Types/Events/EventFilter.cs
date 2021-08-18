// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Event filter class.
    /// </summary>
    public class EventFilter
    {
        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange StartTime { get; set; }

        /// <summary>
        /// Either range between two timestamps or isNull filter condition.
        /// </summary>
        public TimeRange EndTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange ActiveAtTime { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Asset IDs of related equipment that this event relates to.
        /// </summary>
        public IEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// Asset External IDs of related equipment that this event relates to.
        /// </summary>
        public IEnumerable<string> AssetExternalIds { get; set; }

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
        /// Only include events that belong to these datasets.
        /// </summary>
        public IEnumerable<Identity> DataSetIds { get; set; }

        /// <summary>
        /// The source of the asset.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The event subtype
        /// </summary>
        public string Subtype { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }

        /// <summary>
        /// Filter by this (case-sensitive) prefix for the external ID.
        /// </summary>
        public string ExternalIdPrefix { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}