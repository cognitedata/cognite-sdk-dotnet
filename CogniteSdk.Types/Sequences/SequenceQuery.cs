// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Sequence query DTO.
    /// </summary>
    public class SequenceQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on assets with strict matching.
        /// </summary>
        public SequenceFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
