// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk;

namespace CogniteSdk.Assets
{
    public class AssetSearchQueryDto
    {
        /// <summary>
        /// Filter on assets with strict matching.
        /// </summary>
        public AssetFilterDto Filter { get; set; }

        /// <summary>
        /// Limits the number of results to return.
        /// </summary>
        public long? Limit { get; set; }

        /// <summary>
        /// Fulltext search for assets.
        /// </summary>
        public SearchDto Search { get; set; }
    }
}