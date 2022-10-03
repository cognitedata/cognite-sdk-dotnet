// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for fetching models from a data models space.
    /// </summary>
    public class ListModelsQuery
    {
        /// <summary>
        /// Space external id.
        /// </summary>
        public string SpaceExternalId { get; set; }
    }
}
