// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for reading templategroups.
    /// </summary>
    public class TemplateGroup
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
        /// Timestamp this group was created in milliseconds since 01/01/1970
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Timestamp since this group was last updated in milliseconds since 01/01/1970
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <summary>
        /// Optional data set id
        /// </summary>
        public long? DataSetId { get; set; }
    }
}
