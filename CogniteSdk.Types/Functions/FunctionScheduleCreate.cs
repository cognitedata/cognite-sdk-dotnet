// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The functionSchedule write class.
    /// </summary>
    public class FunctionScheduleCreate
    {
        /// <summary>
        /// The name of the functionScheduleCreate.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// External id of the function.
        /// </summary>
        public string FunctionExternalId { get; set; }


        /// <summary>
        /// Description of function schedule.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Cron expression describes when the function should be called.
        /// Use http://www.cronmaker.com to create a cron expression.
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// Input data to the function. This data is passed deserialized
        /// into the function through one of the arguments called data.
        /// </summary>
        public JsonElement Data { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

