using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for nodes.
    /// </summary>
    public class NodeQueryExpression
    {
        /// <summary>
        /// Chain your table expression from this one.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// A reference to a property.
        /// </summary>
        public PropertyIdentifier Through { get; set; }

        /// <summary>
        /// Filter on nodes.
        /// </summary>
        public IDMSFilter Filter { get; set; }
    }

    /// <summary>
    /// Part of graph query.
    /// </summary>
    public class EdgeQueryExpression
    {
        /// <summary>
        /// Chain your table expression from this one.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Maximum number of hops to traverse edges.
        /// </summary>
        public int? MaxHops { get; set; }

        /// <summary>
        /// Whether to traverse edges pointing out of the initial set, or into the initial set.
        /// </summary>
        public bool Outwards { get; set; }

        /// <summary>
        /// Filter on edges.
        /// </summary>
        public IDMSFilter Filter { get; set; }

        /// <summary>
        /// Filter on nodes along the path.
        /// </summary>
        public IDMSFilter NodeFilter { get; set; }

        /// <summary>
        /// Filter on nodes to terminate traversal on.
        /// </summary>
        public IDMSFilter TerminationFilter { get; set; }
    }

    /// <summary>
    /// Part of graph query.
    /// </summary>
    public class QueryExpression
    {
        /// <summary>
        /// Sort on properties.
        /// </summary>
        public IEnumerable<PropertySort> Sort { get; set; }

        /// <summary>
        /// Second sort on properties.
        /// </summary>
        public IEnumerable<PropertySort> PostSort { get; set; }

        /// <summary>
        /// Maximum number of results to retrieve.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Filter on nodes, either this or Edges should be specified, but not both.
        /// </summary>
        public NodeQueryExpression Nodes { get; set; }

        /// <summary>
        /// Filter on edges, either this or Nodes should be specified, but not both.
        /// </summary>
        public EdgeQueryExpression Edges { get; set; }
    }


    /// <summary>
    /// Query to select a property from the result.
    /// </summary>
    public class ModelQueryProperties
    {
        /// <summary>
        /// Reference to a model.
        /// </summary>
        public ModelIdentifier Model { get; set; }
        /// <summary>
        /// Regex patterns to define which properties to return from this model.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
    }

    /// <summary>
    /// Define which model to return properties for and which properties to return
    /// </summary>
    public class SelectModelProperties
    {
        /// <summary>
        /// Models to return properties for.
        /// </summary>
        public Dictionary<string, ModelQueryProperties> Models { get; set; }

        /// <summary>
        /// Optional sort clauses
        /// </summary>
        public IEnumerable<PropertySort> Sort { get; set; }

        /// <summary>
        /// Limit to number of values to return.
        /// </summary>
        public int? Limit { get; set; }
    }

    /// <summary>
    /// Query for nodes and edges.
    /// </summary>
    public class GraphQuery
    {
        /// <summary>
        /// Query
        /// </summary>
        public Dictionary<string, QueryExpression> With { get; set; }

        /// <summary>
        /// Result filter
        /// </summary>
        public Dictionary<string, SelectModelProperties> Select { get; set; }
    }
}
