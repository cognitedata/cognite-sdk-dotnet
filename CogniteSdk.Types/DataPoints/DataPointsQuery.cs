// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// The data points query base class.
    /// </summary>
    public class DataPointsQueryType
    {
        /// <summary>
        /// Get data points starting from, and including, this time. The format is N[timeunit]-ago where time unit is
        /// w,d,h,m,s. Example: '2d-ago' gets data points that are up to 2 days old. You can also specify time in
        /// milliseconds since epoch. Note that for aggregates, the start time is rounded down to a whole granularity
        /// unit (in UTC timezone). Daily granularities (d) are rounded to 0:00 AM; hourly granularities (h) to the
        /// start of the hour, etc.
        /// </summary>
        public string Start  { get; set; }

        /// <summary>
        /// Get data points up to, but excluding, this point in time. Same format as for start. Note that when using
        /// aggregates, the end will be rounded up such that the last aggregate represents a full aggregation interval
        /// containing the original end, where the interval is the granularity unit times the granularity multiplier.
        /// For granularity 2d, the aggregation interval is 2 days, if end was originally 3 days after the start, it
        /// will be rounded to 4 days after the start.
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// Return up to this number of data points. Maximum is 100000 non-aggregated data points and 10000 aggregated
        /// data points.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Specify the aggregates to return, or an empty array if this sub-query should return data points without
        /// aggregation. This value overrides a top-level default aggregates list.
        /// </summary>
        public IEnumerable<string> Aggregates { get; set; }

        /// <summary>
        /// The time granularity size and unit to aggregate over.
        /// </summary>
        public string Granularity { get; set; }

        /// <summary>
        /// Whether to include the last data point before the requested time period, and the first one after. This
        /// option is useful for interpolating data. It is not available for aggregates.
        /// </summary>
        public bool? IncludeOutsidePoints { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsQueryType>(this);
    }

    /// <summary>
    /// The data points query for each individual data point query within the top-level <see cref="DataPointsQuery">DataPointsQuery</see>.
    /// </summary>
    public class DataPointsQueryItem : DataPointsQueryType
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsQueryItem>(this);
    }

    /// <summary>
    /// The top level data points query.
    /// </summary>
    public class DataPointsQuery : DataPointsQueryType
    {
        /// <summary>
        /// Sequence of data point queries of type <see cref="DataPointsQueryItem">DataPointsQueryItem</see>.
        /// </summary>
        public IEnumerable<DataPointsQueryItem> Items { get; set; }

        /// <summary>
        /// If true, then ignore IDs and external IDs that are not found.
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsQuery>(this);
    }
}