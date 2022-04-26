// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for filtering template views.
    /// </summary>
    public class TemplateViewFilterQuery : CursorQueryBase
    {
        /// <summary>
        /// Optional filter
        /// </summary>
        public JsonElement Filter { get; set; }
    }
}
