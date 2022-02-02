// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Delete a list of transformations, optionally ignoring unknown ids.
    /// </summary>
    public class TransformationDelete : ItemsWithoutCursor<Identity>
    {
        /// <summary>
        /// Ignore IDs and external IDs that are not found. Default: false
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }
    }
}
