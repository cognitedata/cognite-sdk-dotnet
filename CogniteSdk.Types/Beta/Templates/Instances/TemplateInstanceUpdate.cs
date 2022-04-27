// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Update collection for field resolvers
    /// </summary>
    public class FieldResolverUpdate : UpdateCollection<Dictionary<string, BaseFieldResolver>, string>
    {
        /// <summary>
        /// Add and remove field resolvers.
        /// </summary>
        /// <param name="add">Field resolvers to add.</param>
        /// <param name="remove">Field resolvers to remove.</param>
        public FieldResolverUpdate(Dictionary<string, BaseFieldResolver> add, IEnumerable<string> remove) : base(add, remove) { }
    }

    /// <summary>
    /// Update for a template instance
    /// </summary>
    public class TemplateInstanceUpdate
    {
        /// <summary>
        /// Update field resolvers, adding or removing values.
        /// </summary>
        public FieldResolverUpdate FieldResolvers { get; set; }
    }
}
