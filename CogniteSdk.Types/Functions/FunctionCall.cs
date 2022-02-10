// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary    >
    /// The functionCall read class.
    /// </summary>
    public class FunctionCall
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }

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
        /// Error object on function error.
        /// </summary>
        public FunctionError Error { get; set; }

        /// <summary>
        /// A server-generated id for the schedule.
        /// </summary>
        public long ScheduleId { get; set; }

        /// <summary>
        /// A server-generated id for the function.
        /// </summary>
        public long FunctionId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

