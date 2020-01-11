// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Types.Assets
{
    public class AssetSearchDto
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whitespace-separated terms to search for in assets. Does a best-effort fuzzy search in relevant fields
        /// (currently name and description) for variations of any of the search terms, and orders results by relevance.
        /// Uses a different search algorithm than the name and description parameters, and will generally give much
        /// better results. Matching and ordering is not guaranteed to be stable over time, and the fields being
        /// searched may be extended.
        /// </summary>
        public string Query { get; set; }
    }
}