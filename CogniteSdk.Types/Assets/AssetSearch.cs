// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// The sequence search DTO.
    /// </summary>
    public class AssetSearch : SearchQuery<AssetFilter, Search> {

        /// <summary>
        /// Create a new empty Asset search DTO with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the AssetSearch.</returns>
        public static AssetSearch Empty ()
        {
            return new AssetSearch {
                Filter=new AssetFilter(),
                Search=new Search()
            };
        }
    }
}