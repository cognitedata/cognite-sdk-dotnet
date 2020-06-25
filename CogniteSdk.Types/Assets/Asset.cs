// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Asset read class.
    /// </summary>
    public class Asset
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
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The source of this asset.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The Id of the asset.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <summary>
        /// InternalId of the root object.
        /// </summary>
        public long RootId { get; set; }

        /// <summary>
        /// External Id of parent asset provided by client. Must be unique within the project.
        /// </summary>
        public string ParentExternalId { get; set; }

        /// <summary>
        /// Aggregated metrics of the asset.
        /// </summary>
        public AssetAggregateResult Aggregates { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Asset dto &&
                   Id == dto.Id;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);

        /// <summary>
        /// A list of labels associated with this asset.
        /// </summary>
        public IEnumerable<CogniteExternalId> Labels { get; set; }

    }
}

