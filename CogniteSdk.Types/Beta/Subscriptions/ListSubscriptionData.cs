// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request to fetch data from a partition.
    /// </summary>
    public class SubscriptionPartitionRequest
    {
        /// <summary>
        /// Partition index to fetch data from. Between 0 and partitions - 1 inclusive.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Position in the partition stream to start fetching data from.
        /// Defaults to start of stream.
        /// </summary>
        public string Cursor { get; set; }
    }

    /// <summary>
    /// A request to list data from a timeseries.
    /// </summary>
    public class ListSubscriptionData
    {
        /// <summary>
        /// Externally provided ID for the subscription.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Pairs of (partition, cursor) to fetch from.
        /// </summary>
        public IEnumerable<SubscriptionPartitionRequest> Partitions { get; set; }
        /// <summary>
        /// Approximate number of results to return accross all partitions. We will batch together
        /// groups of updates, where each group come from the same ingestion request.
        /// Thus, if a single group is large, it may exceed limit, otherwise we will
        /// return up to limit results. To check whether you have reached the end,
        /// do not rely on the count. Instead, check the hasNext field.
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// If partitions.cursor is not set, the default behavior is to start from the beginning
        /// of the stream. InitializeCursors can be used to override this behavior.
        /// 
        /// The format is "N[timeunit]-ago" where timeunit is w,d,h,m (week, day, hour, minute). For instance,
        /// "2d-ago" will give a stream of changes ingested up to 2 days ago. You can also use "now" to jump
        /// straight to the end of the stream.
        /// 
        /// Note that InitializeCursors is not exact, a deviation of some seconds can occur.
        /// </summary>
        public string InitializeCursors { get; set; }
        // <summary>
        // The maximum time to wait for data to arrive, in seconds. As soon as data is available,
        // the request will return immediately with the data. If the timeout is reached while waiting
        // for data, the request will return an empty data response.
        // Default value is 5, range is from 0 to 5.
        // </summary>
        public int? PollTimeoutSeconds { get; set; }
        // <summary>
        // Show the status code for each data point in the response. Good (code = 0) status codes are always
        // omitted.
        // Default value false.
        // </summary>
        public bool? IncludeStatus { get; set; }
        // <summary>
        // Treat data points with a Bad status code as if they do not exist. Set to false to include all data
        // points.
        // Default value true.
        // </summary>
        public bool? IgnoreBadDataPoints { get; set; }
        // <summary>
        // Treat data points with Uncertain status codes as Bad. Set to false to include uncertain data points.
        // Default value true.
        // </summary>
        public bool? TreatUncertainAsBad { get; set; }
    }
}
