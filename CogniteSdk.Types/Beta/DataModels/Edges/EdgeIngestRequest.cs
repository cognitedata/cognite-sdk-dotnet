﻿// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request for ingesting a list of edges into data model storage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EdgeIngestRequest<T> : ItemsWithSpaceExternalId<T> where T : BaseEdge
    {
        /// <summary>
        /// A reference to a model. Consists of an array of spaceExternalId and modelExternalId,
        /// or just [ edge ] or [ node ], which don't belong to any space.
        /// </summary>
        public ModelIdentifier Model { get; set; }

        /// <summary>
        /// If overwrite is enabled, the items in the bulk will completely overwrite existing data.
        /// The default is disabled, which means patch updates will be used:
        /// only specified keys will be overwritten. With overwrite, missing items keys will null out existing data
        /// – assuming the columns are nullable
        /// </summary>
        public bool Overwrite { get; set; } = false;

        /// <summary>
        /// Automatically create start nodes that do not exist
        /// </summary>
        public bool AutoCreateStartNodes { get; set; }

        /// <summary>
        /// Automatically create end nodes that do not exist
        /// </summary>
        public bool AutoCreateEndNodes { get; set; }
    }
}