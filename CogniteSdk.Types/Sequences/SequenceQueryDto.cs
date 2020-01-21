// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The Sequence query DTO.
    /// </summary>
    public class SequenceQueryDto : CursorQueryBase
    {
        /// <summary>
        /// Filter on assets with strict matching.
        /// </summary>
        public SequenceFilterDto Filter { get; set; }
    }
}
