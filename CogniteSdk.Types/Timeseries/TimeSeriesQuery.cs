// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The time series update DTO.
    /// </summary>
    public class TimeSeriesQuery
    {
        /// <summary>
        /// Filter on time series with strict matching.
        /// </summary>
        public TimeSeriesFilterDto Filter { get; set; }

        /// <summary>
        /// Limits the number of results to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Cursor to next result page.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Splits the data set into N partitions. You need to follow the cursors within each partition in order to
        /// receive all the data. Example: 1/10.
        /// </summary>
        public string Partition { get; set; }
    }
}