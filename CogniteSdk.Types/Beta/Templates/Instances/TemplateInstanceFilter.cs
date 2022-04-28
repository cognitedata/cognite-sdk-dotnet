// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Filter for template instances.
    /// </summary>
    public class TemplateInstanceFilter
    {
        /// <summary>
        /// Filter by data set ids
        /// </summary>
        public IEnumerable<long> DataSetIds { get; set; }
        /// <summary>
        /// Filter by template names
        /// </summary>
        public string TemplateNames { get; set; }
        /// <summary>
        /// Filter by resources templates extend.
        /// </summary>
        public IEnumerable<TemplateInstanceExtends> Extends { get; set; }
    }
}
