// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The data points query for each individual data point query within the top-level <see cref="DataPointsQuery">DataPointsQuery</see>.
    /// </summary>
    public class DataPointsLatestQueryItem
    {
        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public string Before { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// The top level data points query.
    /// </summary>
    public class DataPointsLatestQuery
    {
        /// <summary>
        /// Sequence of data point queries of type <see cref="DataPointsLatestQueryItem">DataPointsLatestQueryItem</see>.
        /// </summary>
        public IEnumerable<DataPointsLatestQueryItem> Items { get; set; }

        /// <summary>
        /// If true, then ignore IDs and external IDs that are not found.
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }
    }
}