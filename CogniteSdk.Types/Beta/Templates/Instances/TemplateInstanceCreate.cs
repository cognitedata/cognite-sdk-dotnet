// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Object referencing another resource this template extends
    /// </summary>
    public class TemplateInstanceExtends
    {
        /// <summary>
        /// Resource type
        /// </summary>
        public string ResourceType { get; set; }
        /// <summary>
        /// Resource id
        /// </summary>
        public string ResourceReference { get; set; }
    }

    /// <summary>
    /// Object to create template instances
    /// </summary>
    public class TemplateInstanceCreate
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
        public Dictionary<string, BaseFieldResolver> FieldResolvers { get; set; }
        /// <summary>
        /// Object referencing another resource this template extends
        /// </summary>
        public TemplateInstanceExtends Extends { get; set; }
    }
}
