// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{

    /// <summary>
    /// Class for querying Template Group Versions.
    /// </summary>
    public class TemplateGroupVersionFilter : CursorQueryBase
    {
        /// <summary>
        /// Minimum version of template
        /// </summary>
        public int? MinVersion { get; set; }

        /// <summary>
        /// Maximum version of template
        /// </summary>
        public int? MaxVersion { get; set; }

        /// <summary>
        /// Query with pinned version
        /// </summary>
        public TemplateGroupVersionFilter(int Version)
        {
            MinVersion = Version;
            MaxVersion = Version;
        }
    }
}
