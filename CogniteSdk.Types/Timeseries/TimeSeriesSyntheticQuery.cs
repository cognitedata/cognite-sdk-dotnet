// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Class for single synthetic time series query.
    /// </summary>
    public class TimeSeriesSyntheticQueryItem
    {
        /// <summary>
        /// Query definition
        /// Read about its syntax here https://docs.cognite.com/dev/concepts/resource_types/timeseries.html#synthetic-time-series
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Get data points starting from, and including, this time. The format is N[timeunit]-ago where time unit is
        /// w,d,h,m,s. Example: '2d-ago' gets data points that are up to 2 days old. You can also specify time in
        /// milliseconds since epoch. Note that for aggregates, the start time is rounded down to a whole granularity
        /// unit (in UTC timezone). Daily granularities (d) are rounded to 0:00 AM; hourly granularities (h) to the
        /// start of the hour, etc.
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// Get data points up to, but excluding, this point in time. Same format as for start. Note that when using
        /// aggregates, the end will be rounded up such that the last aggregate represents a full aggregation interval
        /// containing the original end, where the interval is the granularity unit times the granularity multiplier.
        /// For granularity 2d, the aggregation interval is 2 days, if end was originally 3 days after the start, it
        /// will be rounded to 4 days after the start.
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// Return up to this number of data points. Maximum is 10000   
        /// data points.
        /// </summary>
        public int? Limit { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<TimeSeriesSyntheticQueryItem>(this);
    }

    /// <summary>
    /// The list of synthetic queries to perform
    /// </summary>
    public class TimeSeriesSyntheticQuery
    {
        /// <summary>
        /// Sequence of synthetic queries of type <see cref="TimeSeriesSyntheticQueryItem">TimeSeriesSyntheticQueryItem</see>.
        /// </summary>
        public IEnumerable<TimeSeriesSyntheticQueryItem> Items { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<TimeSeriesSyntheticQuery>(this);
    }
}
