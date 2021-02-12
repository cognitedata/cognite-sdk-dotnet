// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0


using System.Collections.Generic;
using CogniteSdk.Types.Common;


namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for reading templategroups.
    /// </summary>
    public class TemplateGroup : CursorQueryBase
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
    }
}
