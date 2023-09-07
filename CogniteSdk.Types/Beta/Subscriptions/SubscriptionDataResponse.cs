// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// The ID of a single timeseries that has been updated.
    /// </summary>
    public class TimeSeriesId
    {
        /// <summary>
        /// Server generated timeseries ID.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Timeseries external ID.
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// A deleted time range.
    /// </summary>
    public class SubscriptionDataDeletes
    {
        /// <summary>
        /// Time in milliseconds since 00:00:00 Thursday, 1 January 1970 Coordinated Universal Time (UTC)
        /// minus leap seconds. Can be negative to define dates before 1970, up to 1900.
        /// 
        /// Start of deleted time range.
        /// </summary>
        public long InclusiveBegin { get; set; }
        /// <summary>
        /// Time in milliseconds since 00:00:00 Thursday, 1 January 1970 Coordinated Universal Time (UTC)
        /// minus leap seconds. Can be negative to define dates before 1970, up to 1900.
        /// 
        /// End of the deleted time range, exclusive.
        /// </summary>
        public long ExclusiveEnd { get; set; }
    }

    /// <summary>
    /// Description of changes to a timeseries.
    /// </summary>
    public class SubscriptionDataUpdate
    {
        /// <summary>
        /// ID of modified timeseries.
        /// </summary>
        public TimeSeriesId TimeSeries { get; set; }
        /// <summary>
        /// List of datapoints inserted into the timeseries,
        /// possibly overwriting existing data.
        /// </summary>
        public IEnumerable<DataPoint> Upserts { get; set; }
        /// <summary>
        /// List of time ranges in which all data points were deleted.
        /// </summary>
        public IEnumerable<SubscriptionDataDeletes> Deletes { get; set; }
    }

    /// <summary>
    /// Changes to timeseries included in a subscription.
    /// </summary>
    public class SubscriptionChanges
    {
        /// <summary>
        /// Timeseries added to the subscription (for this partition).
        /// </summary>
        public IEnumerable<TimeSeriesId> Added { get; set; }
        /// <summary>
        /// Timeseries removed from the subscription (for this partition).
        /// </summary>
        public IEnumerable<TimeSeriesId> Removed { get; set; }
    }

    /// <summary>
    /// Pagination for a partition.
    /// </summary>
    public class PartitionResult
    {
        /// <summary>
        /// Partition index.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Cursor to use in subsequent query, to fetch the next batch of updates.
        /// </summary>
        public string NextCursor { get; set; }
    }

    /// <summary>
    /// Response when retrieving changes for a subscription.
    /// </summary>
    public class SubscriptionDataResponse
    {
        /// <summary>
        /// List of updates from the subscription, sorted by point in time they were applied
        /// to the time series. Every update contains a time series along with a set
        /// of changes to that time series.
        /// </summary>
        public IEnumerable<SubscriptionDataUpdate> Updates { get; set; }
        /// <summary>
        /// If present, this object changes to the subscription definition. The subscription will
        /// start/stop listening to changes from the time series listed here.
        /// 
        /// These changes can be triggered by explicit changes through the Update subscriptions
        /// endpoint, or they can be caused by changes in time series, in that they start/stop
        /// matching the filter for the subscription.
        /// 
        /// Time series are added to these lists when the change takes effect,
        /// which may be later than the actual trigger.
        /// 
        /// The object is partitioned - it will only be present in the response
        /// for the relevant partition, from which the time series was added/removed.
        /// </summary>
        public SubscriptionChanges SubscriptionChanges { get; set; }
        /// <summary>
        /// Which partitions/cursors to use for the next request.
        /// </summary>
        public IEnumerable<PartitionResult> Partitions { get; set; }
        /// <summary>
        /// Whether there is more data available at the time of the query. In rare cases
        /// we may return true, even if there is no data available. In that case, just
        /// continue to query with the updated cursors, and it will eventually return
        /// false.
        /// </summary>
        public bool HasNext { get; set; }
    }
}
