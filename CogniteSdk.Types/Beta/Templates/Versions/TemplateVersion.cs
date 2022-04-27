// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class representing a template group version.
    /// </summary>
    public class TemplateVersion
    {
        /// <summary>
        /// Version of the Template Group
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// Schema of the Template Group
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Conflict mode to use
        /// </summary>
        public TemplateGroupConflictMode ConflictMode { get; set; }

        /// <summary>
        /// Time this version was created in milliseconds since 01/01/1970
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Time this version was last updated in milliseconds since 01/01/1970
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
