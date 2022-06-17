// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// A flexible data models model
    /// </summary>
    public class ModelCreate
    {
        /// <summary>
        /// Model external id
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// True to allow nodes to use this model.
        /// </summary>
        public bool AllowNode { get; set; } = true;
        /// <summary>
        /// True to allow edges to use this model.
        /// </summary>
        public bool AllowEdge { get; set; } = false;
        /// <summary>
        /// List of properties.
        /// </summary>
        public Dictionary<string, ModelProperty> Properties { get; set; }
        /// <summary>
        /// List of models this model extends.
        /// </summary>
        public IEnumerable<IEnumerable<string>> Extends { get; set; }
        /// <summary>
        /// List of indexes in this model.
        /// </summary>
        public ModelIndexes Indexes { get; set; }
        /// <summary>
        /// List of constraints on this model.
        /// </summary>
        public ModelConstraints Constraints { get; set; }
    }

    /// <summary>
    /// Property of a model.
    /// </summary>
    public class ModelProperty
    {
        /// <summary>
        /// Tyhe type of property field. Allowed types are text, json, boolean, float32, float64, int32, int64,
        /// numeric, timestamp, date, geometry, geography, direct_relation.
        /// You may also define array types for any of those types except direct_relation e.g. text[].
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Defines constraint on nullability of the field.
        /// </summary>
        public bool Nullable { get; set; } = true;
        /// <summary>
        /// A reference to a model. Consists of an array of spaceExternalId and modelExternalId,
        /// or just [ edge ] or [ node ], which don't belong to any space.
        /// </summary>
        public IEnumerable<string> TargetModel { get; set; }
    }

    /// <summary>
    /// A flexible data models model
    /// </summary>
    public class Model : ModelCreate { }
}
