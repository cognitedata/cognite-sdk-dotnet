// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
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
        public IEnumerable<double> Max { get; set; }
        /// <summary>
        /// Min value.
        /// </summary>
        public IEnumerable<double> Min { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

