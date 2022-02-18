// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for querying templategroups.
    /// </summary>
    public class TemplateGroupFilter
    {
        /// <summary>
        /// Filter on owners
        /// </summary>
        public IEnumerable<string> Owners { get; set; }
    }
}
