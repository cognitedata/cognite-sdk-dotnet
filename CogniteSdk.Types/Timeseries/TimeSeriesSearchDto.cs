// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The timeseries search DTO.
    /// </summary>
    public class TimeSeriesSearchDto : SearchQueryDto<TimeSeriesFilterDto, SearchDto> {
        /// <summary>
        /// Create a new empty TimeSeries search DTO with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the EventSearchDto.</returns>
        public static TimeSeriesSearchDto Empty ()
        {
            return new TimeSeriesSearchDto {
                Filter=new TimeSeriesFilterDto(),
                Search=new SearchDto()
            };
        }
    }
}