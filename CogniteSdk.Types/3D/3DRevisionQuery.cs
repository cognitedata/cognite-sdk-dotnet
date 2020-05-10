// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevision query class.
    /// </summary>
    public class ThreeDRevisionQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter based on whether or not is has published revisions.
        /// </summary>
        public bool Published { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<ThreeDRevisionQuery>(this);
    }
}