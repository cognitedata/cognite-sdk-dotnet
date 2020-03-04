// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Dto to perform a graph query.
    /// </summary>
    public class RestrictedGraphQueryResult
    {
        /// <summary>
        /// Boolean results of query.
        /// </summary>
        public IEnumerable<bool> BooleanResults { get; set; }

        /// <summary>
        /// Double results of query.
        /// </summary>
        public IEnumerable<double> DoubleResults { get; set; }

        /// <summary>
        /// Vertice results of query.
        /// </summary>
        public IEnumerable<RelationshipLabelDto> Vertices { get; set; }

        /// <summary>
        /// Edges results of query.
        /// </summary>
        public IEnumerable<RelationshipLabelDto> Edges { get; set; }

        /// <summary>
        /// Vertex properties results of query.
        /// </summary>
        public IEnumerable<RelationshipVertexProperty> VertexProperties { get; set; }

        /// <summary>
        /// Long results of query.
        /// </summary>
        public IEnumerable<long> LongResults { get; set; }

        /// <summary>
        /// Integer results of query.
        /// </summary>
        public IEnumerable<int> IntegerResults { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}