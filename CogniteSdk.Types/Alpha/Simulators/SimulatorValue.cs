// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;
using System.Text.Json.Serialization;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Abstract simulator value. Will either be LongValue, DoubleValue or StringValue.
    /// </summary>
    [JsonConverter(typeof(SimulatorValue))]
    public abstract class SimulatorValue
    {
        /// <summary>
        /// Type of simulator value.
        /// </summary>
        public SimulatorValueType Type { get; private set; }

        /// <summary>
        /// Create a double value.
        /// </summary>
        /// <param name="value">Double value to use.</param>
        /// <returns>New value.</returns>
        public static SimulatorValue Create(double value)
        {
            return new SimulatorValue.Double(value);
        }

        /// <summary>
        /// Create a string value.
        /// </summary>
        /// <param name="value">String value to use.</param>
        /// <returns>New value.</returns>
        public static SimulatorValue Create(string value)
        {
            return new SimulatorValue.String(value);
        }

        /// <summary>
        /// Double (float) simulator value.
        /// </summary>
        public sealed class Double : SimulatorValue
        {
            /// <summary>
            /// Initialize new double value.
            /// </summary>
            /// <param name="value">The value to set.</param>
            public Double(double value)
            {
                Type = SimulatorValueType.DOUBLE;
                Value = value;
            }

            /// <summary>
            /// The contained double value.
            /// </summary>
            public double Value { get; set; }

            /// <summary>
            /// Return string representation of the double value.
            /// </summary>
            public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

            /// <summary>
            /// Check if two double values are equal.
            /// </summary>
            public override bool Equals(object obj)
            {
                return obj != null && Value == (obj as Double).Value;
            }

            /// <summary>
            /// Get hash code for the double value.
            /// </summary>
            public override int GetHashCode() => Value.GetHashCode();
        }

        /// <summary>
        /// String simulator value.
        /// </summary>
        public sealed class String : SimulatorValue
        {
            /// <summary>
            /// Initialize new string value.
            /// </summary>
            /// <param name="value">The value to set.</param>
            public String(string value)
            {
                Type = SimulatorValueType.STRING;
                Value = value;
            }

            /// <summary>
            /// The contained string value.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Return string representation of the string value, i.e the string itself.
            /// </summary>
            public override string ToString() => Value;

            /// <summary>
            /// Check if two string values are equal.
            /// </summary>
            public override bool Equals(object obj)
            {
                return obj != null && Value == (obj as String).Value;
            }

            /// <summary>
            /// Get hash code for the string value.
            /// </summary>
            public override int GetHashCode() => Value.GetHashCode();
        }
    }
}
