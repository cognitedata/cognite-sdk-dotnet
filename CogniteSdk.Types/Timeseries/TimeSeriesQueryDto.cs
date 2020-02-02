// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// The time series query DTO.
    /// </summary>
    public class TimeSeriesQueryDto : CursorQueryBase
    {
        /// <summary>
        /// Filter on time series with strict matching.
        /// </summary>
        public TimeSeriesFilterDto Filter { get; set; }

        /// <summary>
        /// Splits the data set into N partitions. You need to follow the cursors within each partition in order to
        /// receive all the data. Example: 1/10.
        /// </summary>
        public string Partition { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<TimeSeriesQueryDto>(this);
    }
}