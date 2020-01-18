using System.Text.Json.Serialization;

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// The aggregate data point DTO.
    /// </summary>
    public class DataPointAggregateDto : DataPointType
    {
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
    }
}