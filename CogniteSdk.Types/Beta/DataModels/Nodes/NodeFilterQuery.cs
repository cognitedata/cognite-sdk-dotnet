// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for subscribing to changes on nodes.
    /// </summary>
    public class NodeSyncQuery : CursorQueryBase
    {
        /// <summary>
        /// Identifier for the space.
        /// </summary>
        public string SpaceExternalId { get; set; }

        /// <summary>
        /// A reference to a model.
        /// </summary>
        public ModelIdentifier Model { get; set; }

        /// <summary>
        /// A complex filter on nodes.
        /// </summary>
        public IDMSFilter Filter { get; set; }
    }

    /// <summary>
    /// Query for filtering nodes.
    /// </summary>
    public class NodeFilterQuery : NodeSyncQuery
    {
        /// <summary>
        /// List of sort clauses, in order of priority.
        /// </summary>
        public IEnumerable<PropertySort> Sort { get; set; }
    }

    /// <summary>
    /// Query for searching for nodes.
    /// </summary>
    public class NodeSearchQuery : NodeSyncQuery
    {
        /// <summary>
        /// Query string that will be parsed and used for search.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Array of properties that you wish to search.
        /// If none are specified, the service will search all text fields.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
    }
}
