// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// The conflict mode to use.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TemplateGroupConflictMode
    {
        /// <summary>
        /// Patch template group
        /// </summary>
        Patch,
        /// <summary>
        /// Update template group
        /// </summary>
        Update,
        /// <summary>
        /// Force update template group
        /// </summary>
        Force
    }


    /// <summary>
    /// Class for querying Template Group Versions.
    /// </summary>
    public class TemplateGroupVersion : CursorQueryBase
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
    }
}
