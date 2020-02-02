// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.TimeSeries
{
    /// <summary>
    /// TimeSeries delete DTO.
    /// </summary>
    public class TimeSeriesDeleteDto : ItemsWithoutCursor<Identity>
    {
        /// <summary>
        /// Ignore IDs and external IDs that are not found.
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<TimeSeriesDeleteDto>(this);
    }
}
