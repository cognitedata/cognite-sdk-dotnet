// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public Dictionary<string, IStreamRecordAggregate> Aggregates { get; set; }
    }

    /// <summary>
    /// Marker interface for stream record aggregate definitions.
    /// </summary>
    [JsonConverter(typeof(StreamRecordAggregateConverter))]
    public interface IStreamRecordAggregate { }

    /// <summary>
    /// Average aggregate. Returns the average value of a numeric property.
    /// </summary>
    public class AvgStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Property to aggregate on. Format: [space, container, property].
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Count aggregate. Returns the count of non-null values for a property.
    /// </summary>
    public class CountStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Property to aggregate on. Format: [space, container, property].
        /// Optional - if not specified, counts all records.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Min aggregate. Returns the minimum value of a property.
    /// </summary>
    public class MinStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Property to aggregate on. Format: [space, container, property].
        /// Also supports ["createdTime"] and ["lastUpdatedTime"] top-level properties.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Max aggregate. Returns the maximum value of a property.
    /// </summary>
    public class MaxStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Property to aggregate on. Format: [space, container, property].
        /// Also supports ["createdTime"] and ["lastUpdatedTime"] top-level properties.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Sum aggregate. Returns the sum of values for a numeric property.
    /// </summary>
    public class SumStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Property to aggregate on. Format: [space, container, property].
        /// </summary>
        public IEnumerable<string> Property { get; set; }
    }

    /// <summary>
    /// Unique values aggregate. Groups records by unique property values.
    /// </summary>
    public class UniqueValuesStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Property to group by. Format: [space, container, property].
        /// Also supports ["space"] top-level property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }

        /// <summary>
        /// Maximum number of unique values to return. Default 10, max 2000.
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, IStreamRecordAggregate> Aggregates { get; set; }
    }

    /// <summary>
    /// Number histogram aggregate. Groups records into numeric range buckets.
    /// </summary>
    public class NumberHistogramStreamRecordAggregate : IStreamRecordAggregate
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
        /// Hard bounds to limit the range of buckets in the histogram.
        /// </summary>
        public NumberHistogramHardBounds HardBounds { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, IStreamRecordAggregate> Aggregates { get; set; }
    }

    /// <summary>
    /// Hard bounds for number histogram aggregate.
    /// </summary>
    public class NumberHistogramHardBounds
    {
        /// <summary>
        /// The lowest number for histogram buckets.
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// The highest number for histogram buckets.
        /// </summary>
        public double? Max { get; set; }
    }

    /// <summary>
    /// Time histogram aggregate. Groups records into time-based buckets.
    /// </summary>
    public class TimeHistogramStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Property to create histogram for. Format: [space, container, property].
        /// Also supports ["createdTime"] and ["lastUpdatedTime"] top-level properties.
        /// </summary>
        public IEnumerable<string> Property { get; set; }

        /// <summary>
        /// Calendar interval for buckets. Examples: "1s", "1m", "1h", "1d", "1w", "1M", "1q", "1y".
        /// Either CalendarInterval or FixedInterval must be specified, but not both.
        /// </summary>
        public string CalendarInterval { get; set; }

        /// <summary>
        /// Fixed interval for buckets. Examples: "30m", "2h", "3d".
        /// Either CalendarInterval or FixedInterval must be specified, but not both.
        /// </summary>
        public string FixedInterval { get; set; }

        /// <summary>
        /// Hard bounds to limit the range of buckets in the histogram.
        /// </summary>
        public TimeHistogramHardBounds HardBounds { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, IStreamRecordAggregate> Aggregates { get; set; }
    }

    /// <summary>
    /// Hard bounds for time histogram aggregate.
    /// </summary>
    public class TimeHistogramHardBounds
    {
        /// <summary>
        /// The lowest time point for histogram buckets (ISO-8601 format).
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// The highest time point for histogram buckets (ISO-8601 format).
        /// </summary>
        public string Max { get; set; }
    }

    /// <summary>
    /// Filters aggregate. Groups records by custom filter conditions.
    /// </summary>
    public class FiltersStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// List of filters to create buckets for.
        /// </summary>
        public IEnumerable<IDMSFilter> Filters { get; set; }

        /// <summary>
        /// Nested sub-aggregates to compute within each bucket.
        /// </summary>
        public Dictionary<string, IStreamRecordAggregate> Aggregates { get; set; }
    }

    /// <summary>
    /// Moving function aggregate. Applies sliding window functions to histogram buckets.
    /// Must be embedded inside a numberHistogram or timeHistogram aggregate.
    /// </summary>
    public class MovingFunctionStreamRecordAggregate : IStreamRecordAggregate
    {
        /// <summary>
        /// Path to the metric to apply the moving function to.
        /// Use "_count" to access count of records in the bucket.
        /// </summary>
        public string BucketsPath { get; set; }

        /// <summary>
        /// Size of the sliding window.
        /// </summary>
        public int Window { get; set; }

        /// <summary>
        /// The moving function to apply.
        /// Supported values: "MovingFunctions.max", "MovingFunctions.min", "MovingFunctions.sum",
        /// "MovingFunctions.unweightedAvg", "MovingFunctions.linearWeightedAvg".
        /// </summary>
        public string Function { get; set; }
    }

    /// <summary>
    /// JSON converter for stream record aggregate types.
    /// </summary>
    public class StreamRecordAggregateConverter : ExtTaggedUnionConverter<IStreamRecordAggregate>
    {
        /// <inheritdoc />
        protected override string TypeSuffix => "StreamRecordAggregate";

        /// <inheritdoc />
        protected override IStreamRecordAggregate DeserializeFromPropertyName(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
        {
            return propertyName switch
            {
                "avg" => JsonSerializer.Deserialize<AvgStreamRecordAggregate>(ref reader, options),
                "count" => JsonSerializer.Deserialize<CountStreamRecordAggregate>(ref reader, options),
                "min" => JsonSerializer.Deserialize<MinStreamRecordAggregate>(ref reader, options),
                "max" => JsonSerializer.Deserialize<MaxStreamRecordAggregate>(ref reader, options),
                "sum" => JsonSerializer.Deserialize<SumStreamRecordAggregate>(ref reader, options),
                "uniqueValues" => JsonSerializer.Deserialize<UniqueValuesStreamRecordAggregate>(ref reader, options),
                "numberHistogram" => JsonSerializer.Deserialize<NumberHistogramStreamRecordAggregate>(ref reader, options),
                "timeHistogram" => JsonSerializer.Deserialize<TimeHistogramStreamRecordAggregate>(ref reader, options),
                "filters" => JsonSerializer.Deserialize<FiltersStreamRecordAggregate>(ref reader, options),
                "movingFunction" => JsonSerializer.Deserialize<MovingFunctionStreamRecordAggregate>(ref reader, options),
                _ => null
            };
        }
    }

    /// <summary>
    /// Response from an aggregation request.
    /// </summary>
    public class StreamRecordsAggregateResponse
    {
        /// <summary>
        /// The aggregation results, keyed by the aggregate identifiers from the request.
        /// </summary>
        public Dictionary<string, IStreamRecordAggregateResult> Aggregates { get; set; }
    }

    /// <summary>
    /// Marker interface for stream record aggregate results.
    /// </summary>
    [JsonConverter(typeof(StreamRecordAggregateResultConverter))]
    public interface IStreamRecordAggregateResult { }

    /// <summary>
    /// Average aggregate result.
    /// </summary>
    public class AvgStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The average value.
        /// </summary>
        public double Avg { get; set; }
    }

    /// <summary>
    /// Count aggregate result.
    /// </summary>
    public class CountStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The count value.
        /// </summary>
        public long Count { get; set; }
    }

    /// <summary>
    /// Min aggregate result.
    /// </summary>
    public class MinStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        public double Min { get; set; }
    }

    /// <summary>
    /// Max aggregate result.
    /// </summary>
    public class MaxStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The maximum value.
        /// </summary>
        public double Max { get; set; }
    }

    /// <summary>
    /// Sum aggregate result.
    /// </summary>
    public class SumStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The sum value.
        /// </summary>
        public double Sum { get; set; }
    }

    /// <summary>
    /// Moving function aggregate result.
    /// </summary>
    public class MovingFunctionStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The moving function result value.
        /// </summary>
        public double FnValue { get; set; }
    }

    /// <summary>
    /// Unique values aggregate result.
    /// </summary>
    public class UniqueValuesStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The unique value buckets.
        /// </summary>
        public IEnumerable<StreamRecordUniqueValueBucket> UniqueValueBuckets { get; set; }
    }

    /// <summary>
    /// Number histogram aggregate result.
    /// </summary>
    public class NumberHistogramStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The number histogram buckets.
        /// </summary>
        public IEnumerable<StreamRecordNumberHistogramBucket> NumberHistogramBuckets { get; set; }
    }

    /// <summary>
    /// Time histogram aggregate result.
    /// </summary>
    public class TimeHistogramStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The time histogram buckets.
        /// </summary>
        public IEnumerable<StreamRecordTimeHistogramBucket> TimeHistogramBuckets { get; set; }
    }

    /// <summary>
    /// Filters aggregate result.
    /// </summary>
    public class FiltersStreamRecordAggregateResult : IStreamRecordAggregateResult
    {
        /// <summary>
        /// The filter buckets.
        /// </summary>
        public IEnumerable<StreamRecordFilterBucket> FilterBuckets { get; set; }
    }

    /// <summary>
    /// JSON converter for stream record aggregate result types.
    /// </summary>
    public class StreamRecordAggregateResultConverter : ExtTaggedUnionConverter<IStreamRecordAggregateResult>
    {
        /// <inheritdoc />
        protected override string TypeSuffix => "StreamRecordAggregateResult";

        /// <inheritdoc />
        protected override IStreamRecordAggregateResult DeserializeFromPropertyName(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
        {
            return propertyName switch
            {
                "avg" => new AvgStreamRecordAggregateResult { Avg = reader.GetDouble() },
                "count" => new CountStreamRecordAggregateResult { Count = reader.GetInt64() },
                "min" => new MinStreamRecordAggregateResult { Min = reader.GetDouble() },
                "max" => new MaxStreamRecordAggregateResult { Max = reader.GetDouble() },
                "sum" => new SumStreamRecordAggregateResult { Sum = reader.GetDouble() },
                "fnValue" => new MovingFunctionStreamRecordAggregateResult { FnValue = reader.GetDouble() },
                "uniqueValueBuckets" => new UniqueValuesStreamRecordAggregateResult { UniqueValueBuckets = JsonSerializer.Deserialize<IEnumerable<StreamRecordUniqueValueBucket>>(ref reader, options) },
                "numberHistogramBuckets" => new NumberHistogramStreamRecordAggregateResult { NumberHistogramBuckets = JsonSerializer.Deserialize<IEnumerable<StreamRecordNumberHistogramBucket>>(ref reader, options) },
                "timeHistogramBuckets" => new TimeHistogramStreamRecordAggregateResult { TimeHistogramBuckets = JsonSerializer.Deserialize<IEnumerable<StreamRecordTimeHistogramBucket>>(ref reader, options) },
                "filterBuckets" => new FiltersStreamRecordAggregateResult { FilterBuckets = JsonSerializer.Deserialize<IEnumerable<StreamRecordFilterBucket>>(ref reader, options) },
                _ => null
            };
        }
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
        public Dictionary<string, IStreamRecordAggregateResult> Aggregates { get; set; }
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
        public Dictionary<string, IStreamRecordAggregateResult> Aggregates { get; set; }
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
        public Dictionary<string, IStreamRecordAggregateResult> Aggregates { get; set; }
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
        public Dictionary<string, IStreamRecordAggregateResult> Aggregates { get; set; }
    }
}
