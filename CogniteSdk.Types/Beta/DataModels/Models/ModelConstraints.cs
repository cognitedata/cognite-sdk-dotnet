// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Constraints on a model.
    /// </summary>
    public class ModelConstraints
    {
        /// <summary>
        /// A collection of uniqueness constraints.
        /// </summary>
        public Dictionary<string, UniqueConstraintProperty> Uniqueness { get; set; }
    }

    /// <summary>
    /// A property in a uniqueness constraint.
    /// </summary>
    public class UniqueConstraintProperty
    {
        /// <summary>
        /// List of properties this constraint consists of.
        /// </summary>
        public IEnumerable<string> UniqueProperties { get; set; }
    }
}
