// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Search text fields in views for nodes or edges. The service will return up to 1000 results.
    /// </summary>
    public class InstancesSearch
    {
        /// <summary>
        /// View to search.
        /// </summary>
        public ViewIdentifier View { get; set; }
        /// <summary>
        /// Search query string.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Limit the search query to nodes or edges. The default is nodes.
        /// </summary>
        public InstanceType? InstanceType { get; set; }
        /// <summary>
        /// Optional array of properties to search. If you do not specify one or more properties,
        /// the service will search all fields in the view.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
        /// <summary>
        /// Optional filter.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// Limits the number of results to return. Default 100.
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// Sort the results
        /// </summary>
        public IEnumerable<InstanceSort> Sort { get; set; }
    }
}
