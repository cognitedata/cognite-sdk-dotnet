// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels
{
    // There is a QuerySetOperationTableExpression in the spec, but I'm not entirely sure what that is.
    // Adding it should simply be adding another case here.

    /// <summary>
    /// Common interface for query expressions.
    /// </summary>
    public interface IQueryTableExpression { }

    /// <summary>
    /// Query nodes
    /// </summary>
    public class QueryNodeTableExpression : IQueryTableExpression
    {
        /// <summary>
        /// Sort returned nodes.
        /// </summary>
        public IEnumerable<InstanceSort> Sort { get; set; }
        /// <summary>
        /// Maximum number of nodes to return.
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// Required query.
        /// </summary>
        public QueryNodes Nodes { get; set; }
    }

    /// <summary>
    /// Query for nodes.
    /// </summary>
    public class QueryNodes
    {
        /// <summary>
        /// Chain result expression based on this query.
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Query through a direct relation.
        /// </summary>
        public QueryViewReference Through { get; set; }
        /// <summary>
        /// Filter
        /// </summary>
        public IDMSFilter Filter { get; set; }
    }

    /// <summary>
    /// Reference to a view and query identifier within a query.
    /// </summary>
    public class QueryViewReference
    {
        /// <summary>
        /// Reference to a view.
        /// </summary>
        public ViewIdentifier View { get; set; }
        /// <summary>
        /// The unique identifier, from the view, for the property.
        /// </summary>
        public string Identifier { get; set; }
    }

    /// <summary>
    /// Query edges
    /// </summary>
    public class QueryEdgeTableExpression : IQueryTableExpression
    {
        /// <summary>
        /// Sort returned edges.
        /// </summary>
        public IEnumerable<InstanceSort> Sort { get; set; }
        /// <summary>
        /// Sort returned edges.
        /// </summary>
        public IEnumerable<InstanceSort> PostSort { get; set; }
        /// <summary>
        /// Maximum number of edges to return.
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// Required query.
        /// </summary>
        public QueryEdges Edges { get; set; }
    }

    /// <summary>
    /// Query expression for edges.
    /// </summary>
    public class QueryEdges
    {
        /// <summary>
        /// Chain result expression based on this query.
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Maximum number of levels to traverse.
        /// </summary>
        public int? MaxDistance { get; set; }
        /// <summary>
        /// Edge direction.
        /// </summary>
        public ConnectionDirection Direction { get; set; }
        /// <summary>
        /// Filter on edges.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// Filter on nodes along the traversed path.
        /// </summary>
        public IDMSFilter NodeFilter { get; set; }
        /// <summary>
        /// Filter on nodes to terminate traversal.
        /// </summary>
        public IDMSFilter TerminationFilter { get; set; }
        /// <summary>
        /// Limit the number of returned edges for each of the source nodes in the result set.
        /// The indicated uniform limit applies to the result set from the referenced from.
        /// limitEach only has meaning when you also specify maxDistance=1 and from.
        /// </summary>
        public int? LimitEach { get; set; }
    }

    /// <summary>
    /// Json converter for the "with" part of queries.
    /// </summary>
    public class QueryTableExpressionConverter : UntaggedUnionConverter<IQueryTableExpression>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QueryTableExpressionConverter() : base(new[]
        {
            typeof(QueryNodeTableExpression), typeof(QueryEdgeTableExpression)
        })
        {
        }
    }
}
