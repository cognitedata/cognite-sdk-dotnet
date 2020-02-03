// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets
{
    /// <summary>
    /// The sequence search DTO.
    /// </summary>
    public class AssetSearchDto : SearchQueryDto<AssetFilterDto, SearchDto> {

        /// <summary>
        /// Create a new empty Asset search DTO with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the AssetSearchDto.</returns>
        public static AssetSearchDto Empty ()
        {
            return new AssetSearchDto {
                Filter=new AssetFilterDto(),
                Search=new SearchDto()
            };
        }
    }
}