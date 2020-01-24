// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets
{
    /// <summary>
    /// Aggregated metrics of the asset.
    /// </summary>
    public class AggregateResult
    {
        /// <summary>
        /// Number of direct descendants for the asset.
        /// </summary>
        public int ChildCount { get; set; }
    }
}
