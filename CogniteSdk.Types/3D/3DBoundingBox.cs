// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDBoundingBox class.
    /// </summary>
    public class ThreeDBoundingBox
    {
        /// <summary>
        /// Max value.
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// Min value.
        /// </summary>
        public double Min { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

