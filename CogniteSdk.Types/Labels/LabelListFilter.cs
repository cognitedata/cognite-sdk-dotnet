// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Filter on label definitions with strict matching.
    /// </summary>
    public class LabelListFilter
    {
        /// <summary>
        /// Returns label definitions with matching name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Returns label definitions with externalIds starting with this.
        /// </summary>
        public string ExternalIdPrefix { get; set; }
        /// <summary>
        /// Return label definitions attached to any of these data set ids.
        /// </summary>
        public IEnumerable<Identity> DataSetIds { get; set; }
    }
}
