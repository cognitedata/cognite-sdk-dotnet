// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for fetching template instances.
    /// </summary>
    public class TemplateInstanceFilterQuery : CursorQueryBase
    {
        /// <summary>
        /// Optional filter
        /// </summary>
        public TemplateInstanceFilter Filter { get; set; }
    }
}
