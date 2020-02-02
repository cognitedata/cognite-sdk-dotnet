// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The sequence search DTO.
    /// </summary>
    public class SequenceSearchDto : SearchQueryDto<SequenceFilterDto, SearchDto> {

        /// <summary>
        /// Create a new pre-initialized Asset search DTO.
        /// </summary>
        /// <returns>New instance of the AssetSearchDto.</returns>
        public static SequenceSearchDto Empty ()
        {
            return new SequenceSearchDto {
                Filter=new SequenceFilterDto(),
                Search=new SearchDto()
            };
        }
    }
}