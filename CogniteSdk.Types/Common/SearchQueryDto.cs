// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk;

namespace CogniteSdk
{
    /// <summary>
    /// Combined search and filter query.
    /// </summary>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <typeparam name="TSearch">Search type</typeparam>
    public class SearchQueryDto<TFilter, TSearch>
    {
        /// <summary>
        /// Filter on items with strict matching.
        /// </summary>
        public TFilter Filter { get; set; }

        /// <summary>
        /// Limits the number of items to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Fulltext search for items.
        /// </summary>
        public TSearch Search { get; set; }
    }
}