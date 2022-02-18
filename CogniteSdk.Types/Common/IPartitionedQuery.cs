// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Types.Common
{
    /// <summary>
    /// Interface for queries with the "Partition" parameter, splitting the data set into N partitions.
    /// </summary>
    public interface IPartitionedQuery
    {
        /// <summary>
        /// Splits the data set into N partitions. You need to follow the cursors within each partition in order to
        /// receive all the data. Example: 1/10.
        /// </summary>
        string Partition { get; set; }
    }
}
