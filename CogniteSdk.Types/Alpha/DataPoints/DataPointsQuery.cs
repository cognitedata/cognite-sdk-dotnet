// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Beta.DataModels;
using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The data points query for each individual data point query within the top-level <see cref="DataPointsQuery">DataPointsQuery</see>.
    /// </summary>
    public class DataPointsQueryItem : CogniteSdk.DataPointsQueryItem
    {
        /// <summary>
        /// The instance ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public InstanceIdentifier InstanceId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsQueryItem>(this);
    }

    /// <summary>
    /// The top level data points query.
    /// </summary>
    public class DataPointsQuery : DataPointsQueryType
    {
        /// <summary>
        /// Sequence of data point queries of type <see cref="DataPointsQueryItem">DataPointsQueryItem</see>.
        /// </summary>
        public IEnumerable<DataPointsQueryItem> Items { get; set; }

        /// <summary>
        /// If true, then ignore IDs, external IDs and instance IDs that are not found.
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsQuery>(this);
    }
}