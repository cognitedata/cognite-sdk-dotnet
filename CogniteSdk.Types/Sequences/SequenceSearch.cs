// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// The sequence search class.
    /// </summary>
    public class SequenceSearch : SearchQuery<SequenceFilter, Search>
    {

        /// <summary>
        /// Create a new pre-initialized Asset search.
        /// </summary>
        /// <returns>New instance of the AssetSearch.</returns>
        public static SequenceSearch Empty()
        {
            return new SequenceSearch
            {
                Filter = new SequenceFilter(),
                Search = new Search()
            };
        }
    }
}