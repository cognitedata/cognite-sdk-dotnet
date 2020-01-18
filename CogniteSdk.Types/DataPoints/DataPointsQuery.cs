// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The data points query base class.
    /// </summary>
    public class DataPointsQueryType
    {
        /// <summary>
        /// Get datapoints starting from, and including, this time. The format is N[timeunit]-ago where timeunit is
        /// w,d,h,m,s. Example: '2d-ago' gets datapoints that are up to 2 days old. You can also specify time in
        /// milliseconds since epoch. Note that for aggregates, the start time is rounded down to a whole granularity
        /// unit (in UTC timezone). Daily granularities (d) are rounded to 0:00 AM; hourly granularities (h) to the
        /// start of the hour, etc.
        /// </summary>
        public long? Start  { get; set; }

        /// <summary>
        /// Get datapoints up to, but excluding, this point in time. Same format as for start. Note that when using
        /// aggregates, the end will be rounded up such that the last aggregate represents a full aggregation interval
        /// containing the original end, where the interval is the granularity unit times the granularity multiplier.
        /// For granularity 2d, the aggregation interval is 2 days, if end was originally 3 days after the start, it
        /// will be rounded to 4 days after the start.
        /// </summary>
        public long? End { get; set; }

        /// <summary>
        /// Return up to this number of datapoints. Maximum is 100000 non-aggregated data points and 10000 aggregated
        /// data points.
        /// </summary>
        public long? Limit { get; set; }

        /// <summary>
        /// Specify the aggregates to return, or an empty array if this sub-query should return datapoints without
        /// aggregation. This value overrides a top-level default aggregates list.
        /// </summary>
        public IEnumerable<string> Aggregates { get; set; }

        /// <summary>
        /// The time granularity size and unit to aggregate over.
        /// </summary>
        public string Granularity { get; set; }

        /// <summary>
        /// Whether to include the last datapoint before the requested time period, and the first one after. This option
        /// is useful for interpolating data. It is not available for aggregates.
        /// </summary>
        /// <value></value>
        public bool? IncludeOutsidePoints { get; set; }
    }

    /// <summary>
    /// The data points query by Id.
    /// </summary>
    public class DataPointsQueryById : DataPointsQueryType
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// The data points query by External Id.
    /// </summary>
    public class DataPointsQueryByExernalId : DataPointsQueryType
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// The data points query.
    /// </summary>
    public class DataPointsQuery : DataPointsQueryType
    {
        /// <summary>
        /// Sequence of DataPointsQueryById or DataPointsQueryByExernalId
        /// </summary>
        public IEnumerable<DataPointsQueryType> Items { get; set; }

        /// <summary>
        /// Ignore IDs and external IDs that are not found.
        /// </summary>
        /// <value></value>
        public bool? IgnoreUnknownIds { get; set; }
    }
}