// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulation run callback class. Used to report the status of a simulation run.
    /// </summary>
    public class SimulationRunCallbackItem
    {
        /// <summary>
        /// The simulation run id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// A new simulation run status to set
        /// </summary>
        public SimulationRunStatus Status { get; set; }

        /// <summary>
        /// A new simulation run status message to set
        /// </summary>
        public string StatusMessage { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRunCallbackItem>(this);
    }
}