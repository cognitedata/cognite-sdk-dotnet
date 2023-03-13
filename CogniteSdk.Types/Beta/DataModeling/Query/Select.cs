// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Expression for "select" part of queries.
    /// </summary>
    public class SelectExpression
    {
        /// <summary>
        /// Sort the resulting values.
        /// </summary>
        public IEnumerable<InstanceSort> Sort { get; set; }
        /// <summary>
        /// Maximum number of results to return.
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// Sources to select from.
        /// </summary>
        public IEnumerable<SelectSource> Sources { get; set; }
    }

    /// <summary>
    /// Source to select from.
    /// </summary>
    public class SelectSource
    {
        /// <summary>
        /// View to select from.
        /// </summary>
        public ViewIdentifier Source { get; set; }
        /// <summary>
        /// Regex patterns used to select the properties to return from the view.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
    }
}
