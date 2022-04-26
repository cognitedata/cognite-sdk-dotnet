// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request to resolve a view.
    /// </summary>
    public class TemplateViewResolveRequest : CursorQueryBase
    {
        /// <summary>
        /// View external id.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// View input.
        /// </summary>
        public Dictionary<string, JsonElement> Input { get; set; }
    }
}
