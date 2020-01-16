// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

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
    }
}
