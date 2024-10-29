// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Slim read type for instances.
    /// </summary>
    public class SlimInstance
    {
        /// <summary>
        /// Type of instance.
        /// </summary>
        public InstanceType InstanceType { get; set; }
        /// <summary>
        /// Current version of this instance.
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Whether or not the edge was modified by this ingestion.
        /// </summary>
        public bool WasModified { get; set; }
        /// <summary>
        /// Id of the space that the node belongs to. This space-id cannot be updated.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Unique identifier for the node.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Time when this view was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this view was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }
    }
}
