// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk;

namespace CogniteSdk
{
    public class SearchQueryDto<TFilter>
    {
        /// <summary>
        /// Filter on items with strict matching.
        /// </summary>
        public TFilter Filter { get; set; }

        /// <summary>
        /// Limits the number of items to return.
        /// </summary>
        public long? Limit { get; set; }

        /// <summary>
        /// Fulltext search for items.
        /// </summary>
        public SearchDto Search { get; set; }
    }
}