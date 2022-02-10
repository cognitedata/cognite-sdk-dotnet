// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary    >
    /// The functionCallResponse read class.
    /// </summary>
    public class FunctionCallResponse
    {
        /// <summary>
        /// The response from the function call as JSON.
        /// </summary>
        public JsonElement Response { get; set; }

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

