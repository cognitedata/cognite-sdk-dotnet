// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Abstract error value. Will either be LongValue, DoubleValue or StringValue.
    /// </summary>
    [JsonConverter(typeof(ValueType))]
    public abstract class ValueType {}

    /// <summary>
    /// Long (int64) error value
    /// </summary>
    public class LongValue : ValueType
    {
        /// <summary>
        /// The contained long value.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Return string representation of the long value.
        /// </summary>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    /// <summary>
    /// Double (float) error value.
    /// </summary>
    public class DoubleValue : ValueType
    {
        /// <summary>
        /// The contained double value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Return string representation of the double value.
        /// </summary>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    /// <summary>
    /// String error value.
    /// </summary>
    public class StringValue : ValueType
    {
        /// <summary>
        /// The contained string value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Return string representation of the string value, i.e the string itself.
        /// </summary>
        public override string ToString()
        {
            return this.Value;
        }
    }
}
