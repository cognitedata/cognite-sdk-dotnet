// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Update for a template instance
    /// </summary>
    public class TemplateInstanceUpdate
    {
        /// <summary>
        /// Update field resolvers, adding or removing values.
        /// </summary>
        public UpdateCollection<Dictionary<string, BaseFieldResolver>, string> FieldResolvers { get; set; }
    }
}
