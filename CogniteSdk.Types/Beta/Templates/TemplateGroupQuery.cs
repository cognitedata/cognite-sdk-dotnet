// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for fetching template groups
    /// </summary>
    public class TemplateGroupQuery : CursorQueryBase
    {
        /// <summary>
        /// Optional filter
        /// </summary>
        public TemplateGroupFilter Filter { get; set; }
    }
}
