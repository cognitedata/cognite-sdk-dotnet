// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.DataModels;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request for aggregating records in a stream.
    /// </summary>
    public class StreamRecordsAggregate
    {
        /// <summary>
        /// Filter on records created within the provided range.
        /// </summary>
        public LastUpdatedTimeFilter LastUpdatedTime { get; set; }

        /// <summary>
        /// A filter Domain Specific Language (DSL) used to create advanced filter queries.
        /// </summary>
        public IDMSFilter Filter { get; set; }

        /// <summary>
        /// A dictionary of requested aggregates with client defined names/identifiers.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateDefinition> Aggregates { get; set; }
    }

    /// <summary>
    /// Base class for aggregate definitions. Use one of the derived classes to specify the aggregate type.
    /// </summary>
    public class StreamRecordAggregateDefinition
    {
        /// <summary>
        /// Average aggregate. Returns the average value of a numeric property.
        /// </summary>
        public StreamRecordPropertyAggregate Avg { get; set; }

        /// <summary>
        /// Count aggregate. Returns the count of non-null values for a property.
        /// </summary>
        public StreamRecordPropertyAggregate Count { get; set; }

        /// <summary>
        /// Min aggregate. Returns the minimum value of a property.
        /// </summary>
        public StreamRecordPropertyAggregate Min { get; set; }

        /// <summary>
        /// Max aggregate. Returns the maximum value of a property.
        /// </summary>
        public StreamRecordPropertyAggregate Max { get; set; }

        /// <summary>
        /// Sum aggregate. Returns the sum of values for a numeric property.
        /// </summary>
        public StreamRecordPropertyAggregate Sum { get; set; }

        /// <summary>
        /// Unique values aggregate. Groups records by unique property values.
        /// </summary>
        public StreamRecordUniqueValuesAggregate UniqueValues { get; set; }

        /// <summary>
        /// Number histogram aggregate. Groups records into numeric range buckets.
        /// </summary>
        public StreamRecordNumberHistogramAggregate NumberHistogram { get; set; }

        /// <summary>
        /// Time histogram aggregate. Groups records into time-based buckets.
        /// </summary>
        public StreamRecordTimeHistogramAggregate TimeHistogram { get; set; }

        /// <summary>
        /// Filters aggregate. Groups records by custom filter conditions.
        /// </summary>
        public StreamRecordFiltersAggregate Filters { get; set; }

        /// <summary>
        /// Moving function aggregate. Applies sliding window functions to histogram buckets.
        /// </summary>
        public StreamRecordMovingFunctionAggregate MovingFunction { get; set; }
    }

    /// <summary>
    /// Property-based aggregate (avg, count, min, max, sum).
    /// </summary>
    public class StreamRecordPropertyAggregate
    {
        /// <summary>
        /// Property to aggregate on. Format: [space, container, property].
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Unique values (terms) aggregate definition.
    /// </summary>
    public class StreamRecordUniqueValuesAggregate
    {
        /// <summary>
        /// Property to group by. Format: [space, container, property].
        /// </summary>
        public IEnumerable<string> Property { get; set; }

        /// <summary>
        /// Maximum number of unique values to return. Default 10, max 1000.
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateDefinition> Aggregates { get; set; }
    }

    /// <summary>
    /// Number histogram aggregate definition.
    /// </summary>
    public class StreamRecordNumberHistogramAggregate
    {
        /// <summary>
        /// Property to create histogram for. Format: [space, container, property].
        /// </summary>
        public IEnumerable<string> Property { get; set; }

        /// <summary>
        /// Interval size for each bucket.
        /// </summary>
        public double? Interval { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateDefinition> Aggregates { get; set; }
    }

    /// <summary>
    /// Time histogram aggregate definition.
    /// </summary>
    public class StreamRecordTimeHistogramAggregate
    {
        /// <summary>
        /// Property to create histogram for. Format: [space, container, property].
        /// </summary>
        public IEnumerable<string> Property { get; set; }

        /// <summary>
        /// Calendar interval for buckets. Examples: "1d", "1h", "1w", "1M".
        /// </summary>
        public string CalendarInterval { get; set; }

        /// <summary>
        /// Fixed interval for buckets. Examples: "30m", "2h".
        /// </summary>
        public string FixedInterval { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateDefinition> Aggregates { get; set; }
    }

    /// <summary>
    /// Filters aggregate definition.
    /// </summary>
    public class StreamRecordFiltersAggregate
    {
        /// <summary>
        /// List of filters to create buckets for.
        /// </summary>
        public IEnumerable<IDMSFilter> Filters { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateDefinition> Aggregates { get; set; }
    }

    /// <summary>
    /// Moving function aggregate definition.
    /// </summary>
    public class StreamRecordMovingFunctionAggregate
    {
        /// <summary>
        /// Path to the metric to apply the moving function to.
        /// </summary>
        public string BucketsPath { get; set; }

        /// <summary>
        /// Size of the sliding window.
        /// </summary>
        public int Window { get; set; }

        /// <summary>
        /// The moving function to apply.
        /// </summary>
        public string Function { get; set; }
    }

    /// <summary>
    /// Response from an aggregation request.
    /// </summary>
    public class StreamRecordsAggregateResponse
    {
        /// <summary>
        /// The aggregation results, keyed by the aggregate identifiers from the request.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateResult> Aggregates { get; set; }
    }

    /// <summary>
    /// Result of an aggregate operation.
    /// Depending on the aggregate type, one of the properties will be populated.
    /// </summary>
    public class StreamRecordAggregateResult
    {
        /// <summary>
        /// Average value result.
        /// </summary>
        public double? Avg { get; set; }

        /// <summary>
        /// Count result.
        /// </summary>
        public long? Count { get; set; }

        /// <summary>
        /// Minimum value result.
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// Maximum value result.
        /// </summary>
        public double? Max { get; set; }

        /// <summary>
        /// Sum value result.
        /// </summary>
        public double? Sum { get; set; }

        /// <summary>
        /// Moving function value result.
        /// </summary>
        public double? FnValue { get; set; }

        /// <summary>
        /// Unique value buckets result.
        /// </summary>
        public IEnumerable<StreamRecordUniqueValueBucket> UniqueValueBuckets { get; set; }

        /// <summary>
        /// Number histogram buckets result.
        /// </summary>
        public IEnumerable<StreamRecordNumberHistogramBucket> NumberHistogramBuckets { get; set; }

        /// <summary>
        /// Time histogram buckets result.
        /// </summary>
        public IEnumerable<StreamRecordTimeHistogramBucket> TimeHistogramBuckets { get; set; }

        /// <summary>
        /// Filter buckets result.
        /// </summary>
        public IEnumerable<StreamRecordFilterBucket> FilterBuckets { get; set; }
    }

    /// <summary>
    /// Bucket for unique values aggregate.
    /// </summary>
    public class StreamRecordUniqueValueBucket
    {
        /// <summary>
        /// The unique value for this bucket.
        /// </summary>
        public JsonElement Value { get; set; }

        /// <summary>
        /// Number of records in this bucket.
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Results of nested sub-aggregates.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateResult> Aggregates { get; set; }
    }

    /// <summary>
    /// Bucket for number histogram aggregate.
    /// </summary>
    public class StreamRecordNumberHistogramBucket
    {
        /// <summary>
        /// The start value of this bucket's interval.
        /// </summary>
        public double IntervalStart { get; set; }

        /// <summary>
        /// Number of records in this bucket.
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Results of nested sub-aggregates.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateResult> Aggregates { get; set; }
    }

    /// <summary>
    /// Bucket for time histogram aggregate.
    /// </summary>
    public class StreamRecordTimeHistogramBucket
    {
        /// <summary>
        /// The start time of this bucket's interval (ISO-8601 format).
        /// </summary>
        public string IntervalStart { get; set; }

        /// <summary>
        /// Number of records in this bucket.
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Results of nested sub-aggregates.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateResult> Aggregates { get; set; }
    }

    /// <summary>
    /// Bucket for filters aggregate.
    /// </summary>
    public class StreamRecordFilterBucket
    {
        /// <summary>
        /// Number of records matching the filter.
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Results of nested sub-aggregates.
        /// </summary>
        public Dictionary<string, StreamRecordAggregateResult> Aggregates { get; set; }
    }
}
