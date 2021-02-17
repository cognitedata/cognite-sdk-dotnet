// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for querying Template Group Versions.
    /// </summary>
    public class TemplateGroupVersion : CursorQueryBase
    {
        /// <summary>
        /// Version of the Template Group
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Schema of the Template Group
        /// </summary>
        public string Schema { get; set; }
    }
}
