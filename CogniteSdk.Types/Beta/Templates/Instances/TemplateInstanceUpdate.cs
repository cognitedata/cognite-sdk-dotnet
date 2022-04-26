// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Update for a template instance
    /// </summary>
    public class TemplateInstanceUpdate
    {
        /// <summary>
        /// Update field resolvers, adding or removing values.
        /// </summary>
        public UpdateCollection<Dictionary<string, JsonElement>, string> FieldResolvers { get; set; }
    }
}
