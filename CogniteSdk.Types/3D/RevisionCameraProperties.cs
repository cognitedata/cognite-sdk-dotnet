// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Camera class.
    /// </summary>
    public class RevisionCameraProperties
    {
        /// <summary>
        /// Initial camera target. List of 3 numbers
        /// </summary>
        public IEnumerable<double> Target { get; set; }
        /// <summary>
        /// Initial camera position. List of 3 numbers.
        /// </summary>
        public IEnumerable<double> Position { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

