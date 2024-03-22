// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// Simulation run data
    /// </summary>
    public class SimulationRunData
    {
        /// <summary>
        /// The simulation run id
        /// </summary>
        public long RunId { get; set; }

        /// <summary>
        /// The simulation inputs
        /// </summary>
        public IEnumerable<SimulatorValueItem> Inputs { get; set; }

        /// <summary>
        /// The simulation outputs
        /// </summary>
        public IEnumerable<SimulatorValueItem> Outputs { get; set; }
    }
}
