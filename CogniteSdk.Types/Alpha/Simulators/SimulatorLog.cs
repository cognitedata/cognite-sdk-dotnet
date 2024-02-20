// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator log data entry.
    /// </summary>
    public class SimulatorLogDataEntry
    {
        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The log level. DEBUG, INFO, WARNING, ERROR, FATAL.
        /// </summary>
        public string Level { get; set; }
    }

    /// <summary>
    /// A Simulator log resource.
    /// </summary>
    public class SimulatorLog {
        /// <summary>
        /// A unique id of a simulator log.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// List of log entries.
        /// </summary>
        public IEnumerable<SimulatorLogDataEntry> Data { get; set; }

        /// <summary>
        /// Data set id of the simulator log.
        /// </summary>
        public long DataSetId { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorLog>(this);
    }
}
