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
        
        /// <summary>
        /// Node type, a direct relation targeting any node.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Node name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Node description
        /// </summary>
        public string Description { get; set; }
    }
}
