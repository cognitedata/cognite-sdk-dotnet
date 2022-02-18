// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The time series query class.
    /// </summary>
    public class TimeSeriesQuery : CursorQueryBase, IPartitionedQuery
    {
        /// <summary>
        /// Filter on time series with strict matching.
        /// </summary>
        public TimeSeriesFilter Filter { get; set; }

        /// <summary>
        /// Splits the data set into N partitions. You need to follow the cursors within each partition in order to
        /// receive all the data. Example: 1/10.
        /// </summary>
        public string Partition { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}