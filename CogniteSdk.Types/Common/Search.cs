// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Search query class. Shared between the various resource search functions.
    /// </summary>
    public class Search
    {
        /// <summary>
        /// Prefix and fuzzy search on name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Prefix and fuzzy search on description..
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whitespace-separated terms to search for in items. Does a best-effort fuzzy search in relevant fields
        /// (currently name and description) for variations of any of the search terms, and orders results by relevance.
        /// Uses a different search algorithm than the name and description parameters, and will generally give much
        /// better results. Matching and ordering is not guaranteed to be stable over time, and the fields being
        /// searched may be extended.
        /// </summary>
        public string Query { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}