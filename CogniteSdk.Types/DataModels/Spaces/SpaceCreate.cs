// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Create a flexible data models space
    /// </summary>
    public class SpaceCreate : SpaceId
    {
        /// <summary>
        /// Description of the space
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Human readable name for the space.
        /// </summary>
        public string Name { get; set; }
    }
}
