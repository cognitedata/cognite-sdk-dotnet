// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// The numeric data point DTO.
    /// </summary>
    public abstract class DataPointType
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Timestamp { get; set; }
    }

    /// <summary>
    /// Datapoint with double value.
    /// </summary>
    public class DataPointNumericDto : DataPointType
    {
        /// <summary>
        /// The data value.
        /// </summary>
        public double Value { get; set; }
    }

    /// <summary>
    /// Datapoint with string value.
    /// </summary>
    public class DataPointStringDto : DataPointType
    {
        /// <summary>
        /// The data value.
        /// </summary>
        public string Value { get; set; }
    }
}