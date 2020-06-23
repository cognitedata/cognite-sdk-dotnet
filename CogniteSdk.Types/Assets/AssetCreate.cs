// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Class for creating a new asset.
    /// </summary>
    public class AssetCreate
    {
        /// <summary>
        /// External Id provided by client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parent ID of the asset.
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Javascript friendly internal ID given to the object.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The source of this asset
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// External Id of parent asset provided by client. Must be unique within the project.
        /// </summary>
        public string ParentExternalId { get; set; }

        /// <summary>
        /// List of labels to associate with the asset.
        /// Currently only available for use in playground.
        /// </summary>
        public IEnumerable<CogniteExternalId> Labels { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

