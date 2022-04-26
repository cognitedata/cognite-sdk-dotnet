// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for template versions.
    /// </summary>
    public class TemplateGroupVersionQuery : CursorQueryBase
    {
        /// <summary>
        /// Optional filter to use.
        /// </summary>
        public TemplateGroupFilter Filter { get; set; }
    }
}
