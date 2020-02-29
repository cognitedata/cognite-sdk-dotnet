// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The aggregate data point DTO.
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointAggregate>(this);
    }
}