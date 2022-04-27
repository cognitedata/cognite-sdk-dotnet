// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Request to create a new configuration revision for an extraction pipeline.
    /// </summary>
    public class ExtPipeConfigCreate
    {
        /// <summary>
        /// ExternalId of the extraction pipeline this configuration belongs to.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Raw configuration text.
        /// </summary>
        public string Config { get; set; }
        /// <summary>
        /// Description of this revision.
        /// </summary>
        public string Description { get; set; }
    }
}
