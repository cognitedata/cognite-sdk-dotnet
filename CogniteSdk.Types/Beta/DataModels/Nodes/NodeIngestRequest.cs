// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request for ingesting a list of nodes into data model storage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NodeIngestRequest<T> : ItemsWithSpaceExternalId<T> where T : BaseNode
    {
        /// <summary>
        /// A reference to a model. Consists of an array of spaceExternalId and modelExternalId,
        /// or just [ edge ] or [ node ], which don't belong to any space.
        /// </summary>
        public IEnumerable<string> Model { get; set; }

        /// <summary>
        /// If overwrite is enabled, the items in the bulk will completely overwrite existing data.
        /// The default is disabled, which means patch updates will be used:
        /// only specified keys will be overwritten. With overwrite, missing items keys will null out existing data
        /// – assuming the columns are nullable
        /// </summary>
        public bool Overwrite { get; set; } = false;
    }
}
