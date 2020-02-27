// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The timeseries search DTO.
    /// </summary>
    public class TimeSeriesSearch : SearchQueryDto<TimeSeriesFilter, Search> {
        /// <summary>
        /// Create a new empty TimeSeries search DTO with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the EventSearch.</returns>
        public static TimeSeriesSearch Empty ()
        {
            return new TimeSeriesSearch {
                Filter=new TimeSeriesFilter(),
                Search=new Search()
            };
        }
    }
}