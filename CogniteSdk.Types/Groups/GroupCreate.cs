// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Create a CDF group.
    /// </summary>
    public class GroupCreate
    {
        /// <summary>
        /// Name of the group.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ID of the group in the source. 
        /// If this is the same ID as a group in the IdP,
        /// a service account in that group will implicitly be a part of this group as well.
        /// </summary>
        public string SourceId { get; set; }
        /// <summary>
        /// List of capabilities for this group.
        /// </summary>
        public IEnumerable<BaseAcl> Capabilities { get; set; }
    }
}
