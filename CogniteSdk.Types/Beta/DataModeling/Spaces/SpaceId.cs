// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Identifier for a flexible data models space.
    /// </summary>
    public class SpaceId
    {
        /// <summary>
        /// The space identifier.
        /// Note that certain space ids are reserved:
        /// cdf, dms, pg3, shared, system, node, and edge.
        /// </summary>
        public string Space { get; set; }
    }
}
