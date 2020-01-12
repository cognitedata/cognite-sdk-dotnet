// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Types.Assets
{
    public class AssetReadDto
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
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// The source of this asset.
        /// </summary>
        /// <value></value>
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
        /// <value></value>
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
        public Aggregates Aggregates { get; set; }
    }
}

