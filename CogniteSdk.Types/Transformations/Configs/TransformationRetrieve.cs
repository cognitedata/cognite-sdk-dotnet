// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Object for retrieving transformations.
    /// </summary>
    public class TransformationRetrieve : ItemsWithIgnoreUnknownIds<Identity>
    {
        /// <summary>
        /// Whether the transformations will be returned with last running and last created job details.
        /// </summary>
        public bool WithJobDetails { get; set; }
    }
}
