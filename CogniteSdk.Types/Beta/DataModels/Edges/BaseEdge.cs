﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Base class for an edge.
    /// </summary>
    public class BaseEdge
    {
        /// <summary>
        /// Identifier for the edge.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// A reference to another node.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }

        /// <summary>
        /// A reference to a another node, as start node.
        /// </summary>
        public DirectRelationIdentifier StartNode { get; set; }

        /// <summary>
        /// A reference to another node, as end node.
        /// </summary>
        public DirectRelationIdentifier EndNode { get; set; }
    }
}