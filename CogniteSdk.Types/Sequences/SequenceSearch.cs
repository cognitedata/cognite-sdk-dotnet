// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The sequence search DTO.
    /// </summary>
    public class SequenceSearch : SearchQueryDto<SequenceFilter, Search> {

        /// <summary>
        /// Create a new pre-initialized Asset search DTO.
        /// </summary>
        /// <returns>New instance of the AssetSearch.</returns>
        public static SequenceSearch Empty ()
        {
            return new SequenceSearch {
                Filter=new SequenceFilter(),
                Search=new Search()
            };
        }
    }
}