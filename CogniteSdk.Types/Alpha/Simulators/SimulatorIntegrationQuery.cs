// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The simulator integration query class.
    /// </summary>
    public class SimulatorIntegrationQuery
    {
        /// <summary>
        /// Filter simulators by this.
        /// </summary>
        public SimulatorIntegrationFilter Filter { get; set; }
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorIntegrationQuery>(this);
    }


    /// <inheritdoc />
    public class SimulatorIntegrationFilter
    {
        /// <summary>
        /// Filter by simulator external ids.
        /// </summary>
        public IEnumerable<string> SimulatorExternalIds { get; set; }

        /// <summary>
        /// Filter by whether the simulator integration is active or not.
        /// Connector is considered active if the heartbeat was reported within the last minute.
        /// </summary>
        public bool? Active { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorIntegrationFilter>(this);
    }
}
