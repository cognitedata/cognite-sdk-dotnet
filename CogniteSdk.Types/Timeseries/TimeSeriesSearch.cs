// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// The timeseries search type.
    /// </summary>
    public class TimeSeriesSearch : SearchQuery<TimeSeriesFilter, Search> {
        /// <summary>
        /// Create a new empty TimeSeries search with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the TimeSeriesSearch.</returns>
        public static TimeSeriesSearch Empty ()
        {
            return new TimeSeriesSearch {
                Filter=new TimeSeriesFilter(),
                Search=new Search()
            };
        }
    }
}