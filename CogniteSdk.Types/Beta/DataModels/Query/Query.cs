// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Query accross nodes and edges of a project using a graph approach.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Queries with filters for returned nodes and edges.
        /// </summary>
        public Dictionary<string, IQueryTableExpression> With { get; set; }
        /// <summary>
        /// Groups of values to return.
        /// </summary>
        public Dictionary<string, SelectExpression> Select { get; set; }
        /// <summary>
        /// Parameter values for the query.
        /// </summary>
        public Dictionary<string, IDMSValue> Parameters { get; set; }
    }

    /// <summary>
    /// Subscribe to changes for nodes and edges in a project, matching a supplied filter.
    /// </summary>
    public class SyncQuery : Query
    {
        /// <summary>
        /// Cursors returned from previous sync request.
        /// </summary>
        public Dictionary<string, string> Cursors { get; set; }
    }

    /// <summary>
    /// Result from a sync operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncResult<T> : ItemsWithoutCursor<Dictionary<string, BaseInstance<T>>>
    {
        /// <summary>
        /// Returned cursors.
        /// </summary>
        public Dictionary<string, string> NextCursor { get; set; }
    }
}
