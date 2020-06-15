// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary    >
    /// The functionCallFilter read class.
    /// </summary>
    public class FunctionCallFilter
    {
        /// <summary>
        /// Enum: "Running" "Completed" "Failed" "Timeout"
        /// Status of the function call.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday,
        /// 1 January 1970, Coordinated Universal Time (UTC),
        /// minus leap seconds.
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday,
        /// 1 January 1970, Coordinated Universal Time (UTC),
        /// minus leap seconds.
        /// </summary>
        public long EndTime { get; set; }

        /// <summary>
        /// A server-generated id for the schedule.
        /// </summary>
        public CogniteServerId ScheduleId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

