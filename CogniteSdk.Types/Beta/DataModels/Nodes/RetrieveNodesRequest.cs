// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request to retrieve a list of nodes from a given space and model.
    /// </summary>
    public class RetrieveNodesRequest : ItemsWithSpaceExternalId<CogniteExternalId>
    {
        /// <summary>
        /// Identifier of model to retrieve nodes from.
        /// </summary>
        public ModelIdentifier Model { get; set; }
    }
}
