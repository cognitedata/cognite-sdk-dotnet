// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Abstract error value. Will either be LongValue, DoubleValue or StringValue.
    /// </summary>
    [JsonConverter(typeof(MultiValue))]
    public abstract class MultiValue
    {
        /// <summary>
        /// Type of multi value.
        /// </summary>
        public MultiValueType Type { get; private set; }

        /// <summary>
        /// Create a double value.
        /// </summary>
        /// <param name="value">Double value to use.</param>
        /// <returns>New value.</returns>
        public static MultiValue Create(double value)
        {
            return new MultiValue.Double(value);
        }

        /// <summary>
        /// Create a string value.
        /// </summary>
        /// <param name="value">String value to use.</param>
        /// <returns>New value.</returns>
        public static MultiValue Create(string value)
        {
            return new MultiValue.String(value);
        }

        /// <summary>
        /// Create a long value.
        /// </summary>
        /// <param name="value">String value to use.</param>
        /// <returns>New value.</returns>
        public static MultiValue Create(long value)
        {
            return new MultiValue.Long(value);
        }

        /// <summary>
        /// Long (int64) error value
        /// </summary>
        public sealed class Long : MultiValue
        {
            /// <summary>
            /// Initialize new long value.
            /// </summary>
            /// <param name="value">The value to set.</param>
            public Long(long value)
            {
                Type = MultiValueType.LONG;
                Value = value;
            }

            /// <summary>
            /// The contained long value.
            /// </summary>
            public long Value { get; set; }

            /// <summary>
            /// Return string representation of the long value.
            /// </summary>
            public override string ToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Double (float) error value.
        /// </summary>
        public sealed class Double : MultiValue
        {
            /// <summary>
            /// Initialize new double value.
            /// </summary>
            /// <param name="value">The value to set.</param>
            public Double(double value)
            {
                Type = MultiValueType.DOUBLE;
                Value = value;
            }

            /// <summary>
            /// The contained double value.
            /// </summary>
            public double Value { get; set; }

            /// <summary>
            /// Return string representation of the double value.
            /// </summary>
            public override string ToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// String error value.
        /// </summary>
        public sealed class String : MultiValue
        {
            /// <summary>
            /// Initialize new string value.
            /// </summary>
            /// <param name="value">The value to set.</param>
            public String(string value)
            {
                Type = MultiValueType.STRING;
                Value = value;
            }

            /// <summary>
            /// The contained string value.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Return string representation of the string value, i.e the string itself.
            /// </summary>
            public override string ToString()
            {
                return Value;
            }
        }
    }
}
