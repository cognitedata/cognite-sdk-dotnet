// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Query for aggregates across instance properties.
    /// </summary>
    public class InstancesAggregate : CursorQueryBase
    {
        /// <summary>
        /// Optional query string.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Optional list of propertes to search. If you do not list any properties, we search
        /// all text fields in the view.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
        /// <summary>
        /// List of aggregates to retrieve. Maximum 5.
        /// </summary>
        public IEnumerable<BaseAggregate> Aggregates { get; set; }
        /// <summary>
        /// Selection of fields to group the results by when doing aggregations.
        /// Up to 5 groups.
        /// </summary>
        public IEnumerable<string> GroupBy { get; set; }
        /// <summary>
        /// Optional filter.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// Type of instance. Defaults to node.
        /// </summary>
        public InstanceType? InstanceType { get; set; }
        /// <summary>
        /// View to query.
        /// </summary>
        public ViewIdentifier View { get; set; }
    }

    /// <summary>
    /// Base class for aggregates.
    /// </summary>
    public abstract class BaseAggregate
    {
        /// <summary>
        /// Property to use for aggregation.
        /// </summary>
        public string Property { get; set; }
    }

    /// <summary>
    /// Calculate the average of a property.
    /// </summary>
    public class AvgAggregate : BaseAggregate { }

    /// <summary>
    /// Calculate the count of a property.
    /// </summary>
    public class CountAggregate : BaseAggregate { }

    /// <summary>
    /// Calculate the minimum value of a property.
    /// </summary>
    public class MinAggregate : BaseAggregate { }

    /// <summary>
    /// Calculate the maximum value of a property.
    /// </summary>
    public class MaxAggregate : BaseAggregate { }

    /// <summary>
    /// Calculate the sum of all values of a property.
    /// </summary>
    public class SumAggregate : BaseAggregate { }

    /// <summary>
    /// Calculate a histogram from the property.
    /// </summary>
    public class HistogramAggregate : BaseAggregate
    {
        /// <summary>
        /// The interval between each bucket.
        /// </summary>
        public double Interval { get; set; }
    }

    /// <summary>
    /// Json converter for aggregate types.
    /// </summary>
    public class AggregateConverter : ExtTaggedUnionConverter<BaseAggregate>
    {
        /// <inheritdoc />
        protected override string TypeSuffix => "Average";

        /// <inheritdoc />
        protected override BaseAggregate DeserializeFromPropertyName(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
        {
            BaseAggregate aggregate = null;
            switch (propertyName)
            {
                case "avg": aggregate = JsonSerializer.Deserialize<AvgAggregate>(ref reader, options); break;
                case "count": aggregate = JsonSerializer.Deserialize<CountAggregate>(ref reader, options); break;
                case "min": aggregate = JsonSerializer.Deserialize<MinAggregate>(ref reader, options); break;
                case "max": aggregate = JsonSerializer.Deserialize<MaxAggregate>(ref reader, options); break;
                case "sum": aggregate = JsonSerializer.Deserialize<SumAggregate>(ref reader, options); break;
                case "histogram": aggregate = JsonSerializer.Deserialize<HistogramAggregate>(ref reader, options); break;
            }
            return aggregate;
        }
    }

    /// <summary>
    /// Type of aggregate result
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AggregateType
    {
        /// <summary>
        /// Average aggregate
        /// </summary>
        avg,
        /// <summary>
        /// Minimum aggregate
        /// </summary>
        min,
        /// <summary>
        /// Maximum aggregate
        /// </summary>
        max,
        /// <summary>
        /// Count aggregate
        /// </summary>
        count,
        /// <summary>
        /// Sum aggregate
        /// </summary>
        sum,
        /// <summary>
        /// Histogram aggregate
        /// </summary>
        histogram
    }

    /// <summary>
    /// Base class for aggregate results.
    /// </summary>
    public abstract class BaseAggregateResult
    {
        /// <summary>
        /// Type of aggregate.
        /// </summary>
        public AggregateType Aggregate { get; set; }
        /// <summary>
        /// The property the aggregate was calculated from.
        /// </summary>
        public string Property { get; set; }
    }

    /// <summary>
    /// Aggregate returning a number.
    /// </summary>
    public class NumberAggregateResult : BaseAggregateResult
    {
        /// <summary>
        /// Value of aggregate result.
        /// </summary>
        public double Value { get; set; }
    }

    /// <summary>
    /// Aggregate returning a histogram.
    /// </summary>
    public class HistogramAggregateResult : BaseAggregateResult
    {
        /// <summary>
        /// Configured histogram interval.
        /// </summary>
        public double Interval { get; set; }
        /// <summary>
        /// Returned aggregates.
        /// </summary>
        public IEnumerable<HistogramAggregateBucket> Aggregates { get; set; }
    }

    /// <summary>
    /// A single histogram bucket result.
    /// </summary>
    public class HistogramAggregateBucket
    {
        /// <summary>
        /// Start value for histogram bucket.
        /// </summary>
        public double Start { get; set; }
        /// <summary>
        /// Number of items in the histogram bucket.
        /// </summary>
        public long Count { get; set; }
    }

    /// <summary>
    /// JsonConverter for aggregate result variants.
    /// </summary>
    public class AggregateResultTypeConverter : IntTaggedUnionConverter<BaseAggregateResult, AggregateType>
    {
        /// <inheritdoc />
        protected override string TypePropertyName => "aggregate";

        /// <inheritdoc />
        protected override BaseAggregateResult DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, AggregateType type)
        {
            switch (type)
            {
                case AggregateType.histogram:
                    return document.Deserialize<HistogramAggregateResult>(options);
                default:
                    return document.Deserialize<NumberAggregateResult>(options);
            }
        }
    }

    /// <summary>
    /// Result item when querying aggregates.
    /// </summary>
    public class InstancesAggregateResultItem
    {
        /// <summary>
        /// Type of instance.
        /// </summary>
        public InstanceType InstanceType { get; set; }
        /// <summary>
        /// Group this item belongs to.
        /// </summary>
        public Dictionary<string, IDMSValue> Group { get; set; }
        /// <summary>
        /// List of up to 5 aggregates for this group.
        /// </summary>
        public IEnumerable<BaseAggregateResult> Aggregates { get; set; }
    }

    /// <summary>
    /// Response when requesting instance aggregates.
    /// </summary>
    public class InstancesAggregateResponse : ItemsWithCursor<InstancesAggregateResultItem>
    {
        /// <summary>
        /// Type information for the returned properties.
        /// </summary>
        public Dictionary<string, ContainerPropertyDefinition> Typing { get; set; }
    }
}
