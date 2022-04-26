// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Source of a template view
    /// </summary>
    public class TemplateViewSource
    {
        /// <summary>
        /// Template field mappings
        /// </summary>
        public Dictionary<string, string> Mappings { get; set; }
        /// <summary>
        /// View type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Filter of type given by "Type".
        /// </summary>
        public JsonElement Filter { get; set; }
    }

    /// <summary>
    /// Create a template view
    /// </summary>
    public class TemplateViewCreate
    {
        /// <summary>
        /// External id
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Optional data set id.
        /// </summary>
        public long? DataSetId { get; set; }
    }
}
