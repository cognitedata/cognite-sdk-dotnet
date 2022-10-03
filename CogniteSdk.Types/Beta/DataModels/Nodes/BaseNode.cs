// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Base of a node ingested into CDF.
    /// </summary>
    public class BaseNode
    {
        /// <summary>
        /// Node externalId, required.
        /// </summary>
        public string ExternalId { get; set; }
    }
}
