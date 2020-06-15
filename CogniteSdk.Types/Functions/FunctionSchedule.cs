// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The functionSchedule read class.
    /// </summary>
    public class FunctionSchedule
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The name of the functionSchedule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// External id of the function.
        /// </summary>
        public string FunctionExternalId { get; set; }


        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970,
        /// Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long CreatedTime { get; set; }

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
