// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The time series filter DTO.
    /// </summary>
    public class TimeSeriesFilterDto
    {
        /// <summary>
        /// The name of the time series.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unit of the time series.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Filter on isStep..
        /// </summary>
        public bool? IsStep { get; set; }

        /// <summary>
        /// Filter on isString.
        /// </summary>
        public bool? IsString { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Filter out time series that are not linked to any of these assets.
        /// </summary>
        public IEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// Asset External IDs of related equipment that this time series relates to.
        /// </summary>
        public IEnumerable<string> ExternalAssetIds { get; set; }

        /// <summary>
        /// Only include time series that have a related asset in a tree rooted at any of these root assetIds.
        /// </summary>
        public IEnumerable<long> RootAssetIds { get; set; }

        /// <summary>
        /// Only include assets in subtrees rooted at the specified assets (including the roots given). If the total
        /// size of the given subtrees exceeds 100,000 assets, an error will be returned.
        /// </summary>
        public IEnumerable<Identity> AssetSubtreeIds { get; set; }

        /// <summary>
        /// Filter by this (case-sensitive) prefix for the external ID.
        /// </summary>
        public string ExternalIdPrefix { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<TimeSeriesFilterDto>(this);
    }
}