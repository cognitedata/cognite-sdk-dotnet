// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class representing a template instance
    /// </summary>
    public class TemplateInstance
    {
        /// <summary>
        /// External id.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Name of template
        /// </summary>
        public string TemplateName { get; set; }
        /// <summary>
        /// Optional data set id.
        /// </summary>
        public long? DataSetId { get; set; }
        /// <summary>
        /// Map of field resolvers for this instance
        /// </summary>
        public Dictionary<string, JsonElement> FieldResolvers { get; set; }
        /// <summary>
        /// Object referencing another resource this template extends
        /// </summary>
        public TemplateInstanceExtends Extends { get; set; }

        /// <summary>
        /// Time this instance was created in milliseconds since 01/01/1970
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Time this instance was last updated in milliseconds since 01/01/1970
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
