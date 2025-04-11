// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
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

        /// <summary>
        /// Simulation time in milliseconds. This is the timestamp used for indexing inputs and outputs of the simulation.
        /// </summary>
        public long? SimulationTime { get; set; }

        /// <summary>
        /// The simulation run inputs
        /// </summary>
        public IEnumerable<SimulatorValueItem> Inputs { get; set; }

        /// <summary>
        /// The simulation run outputs
        /// </summary>
        public IEnumerable<SimulatorValueItem> Outputs { get; set; }


        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRunCallbackItem>(this);
    }
}
