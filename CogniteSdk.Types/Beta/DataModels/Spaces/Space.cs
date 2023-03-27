// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// A flexible data models space
    /// </summary>
    public class Space : SpaceId
    {
        /// <summary>
        /// Description of the space
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Human readable name for the space.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Time when this space was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this space was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }
    }
}
