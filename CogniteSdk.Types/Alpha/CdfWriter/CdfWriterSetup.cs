// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A single mapping item for the CDF writer.
    /// </summary>
    public class CdfWriterSetupItem
    {
        /// <summary>
        /// Name of the extractor task this mapping applies to.
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// Type of mapping, for example "space_routing". The special type "custom"
        /// uses <see cref="Destination"/> and <see cref="Fields"/>.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Type specific parameters. Passed through to the CDF writer as-is.
        /// </summary>
        public JsonElement? Parameters { get; set; }
        /// <summary>
        /// Destination endpoint, for example "/timeseries/update".
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// Map from target field to expression, for custom mappings.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Fields { get; set; }
    }

    /// <summary>
    /// Request to configure the CDF writer for an integration.
    /// </summary>
    public class CdfWriterSetupRequest
    {
        /// <summary>
        /// External ID of the integration the mapping belongs to.
        /// </summary>
        public string IntegrationId { get; set; }
        /// <summary>
        /// External ID of the extractor type, for example "cognite-opcua".
        /// </summary>
        public string ExtractorType { get; set; }
        /// <summary>
        /// Version of the extractor.
        /// </summary>
        public string ExtractorVersion { get; set; }
        /// <summary>
        /// Mapping items.
        /// </summary>
        public IEnumerable<CdfWriterSetupItem> Items { get; set; }
    }
}
