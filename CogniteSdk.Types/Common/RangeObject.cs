// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Range between two numbers.
    /// </summary>
    public class RangeObject
    {
        /// <summary>
        /// Max allowable number.
        /// </summary>

        public long? Max { get; set; }
        /// <summary>
        /// Min allowable number.
        /// </summary>
        public long? Min { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Range between two numbers, float.
    /// </summary>
    public class RangeFloat
    {
        /// <summary>
        /// Max allowable number.
        /// </summary>
        public float? Max { get; set; }

        /// <summary>
        /// Min allowable number.
        /// </summary>
        public float? Min { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}