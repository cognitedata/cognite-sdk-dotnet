// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// List labels matching optional filter. Supports pagination.
    /// </summary>
    public class LabelQuery : CursorQueryBase
    {
        /// <summary>
        /// Optional filter on label definitions with strict matching.
        /// </summary>
        public LabelListFilter Filter { get; set; }
    }
}
