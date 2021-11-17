// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// CDF Group object, contains a list of capabilities.
    /// </summary>
    public class Group
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
        /// <summary>
        /// Internal id of group.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// True if this group is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Time this group was deleted from the source.
        /// </summary>
        public long? DeletedTime { get; set; }
    }
}
