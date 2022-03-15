// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Filter for listing transformations.
    /// </summary>
    public class TransformationFilterQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on transformation configurations.
        /// </summary>
        public TransformationFilter Filter { get; set; }
    }
}
