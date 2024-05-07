// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The aggregate data point class.
    /// </summary>
    public class DataPointAggregate
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The integral average value in the aggregate period.
        /// </summary>
        public double Average { get; set; }

        /// <summary>
        /// The maximum value in the aggregate period.
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// The minimum value in the aggregate period.
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// The number of datapoints in the aggregate period.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The sum of the datapoints in the aggregate period.
        /// </summary>
        public double Sum { get; set; }

        /// <summary>
        /// The interpolated value of the series in the beginning of the aggregate.
        /// </summary>
        public double Interpolation { get; set; }

        /// <summary>
        /// The last value before or at the beginning of the aggregate.
        /// </summary>
        public double StepInterpolation { get; set; }

        /// <summary>
        /// The variance of the interpolated underlying function.
        /// </summary>
        public double ContinuousVariance { get; set; }

        /// <summary>
        /// The variance of the datapoint values.
        /// </summary>
        /// <value></value>
        public double DiscreteVariance { get; set; }

        /// <summary>
        /// The total variation of the interpolated underlying function.
        /// </summary>
        public double TotalVariation { get; set; }

        /// <summary>
        /// The total number of data points in the aggregate period that have a Good status code.
        /// Uncertain does not count, irrespective of the treatUncertainAsBad parameter.
        /// </summary>
        public int CountGood { get; set; }

        /// <summary>
        /// The number of data points in the aggregate period that have an Uncertain status code.
        /// </summary>
        public int CountUncertain { get; set; }

        /// <summary>
        /// The number of data points in the aggregate period that have a Bad status code.
        /// Uncertain does not count, irrespective of treatUncertainAsBad parameter.
        /// </summary>
        public int CountBad { get; set; }

        /// <summary>
        /// The duration the aggregate is defined and marked as good (regardless of ignoreBadDataPoints parameter).
        /// Measured in milliseconds. Equivalent to duration that the previous data point is good and in range.
        /// </summary>
        public int DurationGood { get; set; }

        /// <summary>
        /// The duration the aggregate is defined and marked as uncertain (regardless of ignoreBadDataPoints parameter).
        /// Measured in milliseconds. Equivalent to duration that the previous data point is uncertain and in range.
        /// </summary>
        public int DurationUncertain { get; set; }

        /// <summary>
        /// The duration the aggregate is defined but marked as bad (regardless of ignoreBadDataPoints parameter).
        /// Measured in milliseconds. Equivalent to duration that the previous data point is bad and in range.
        /// </summary>
        public int DurationBad { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointAggregate>(this);
    }
}
