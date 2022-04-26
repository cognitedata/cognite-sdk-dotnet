// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for writing templategroups.
    /// </summary>
    public class TemplateGroupCreate
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The description of the Template Group
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The owners of the Template Group.
        /// </summary>
        public IEnumerable<string> Owners { get; set; }

        /// <summary>
        /// Optional data set id
        /// </summary>
        public long? DataSetId { get; set; }
    }
}
