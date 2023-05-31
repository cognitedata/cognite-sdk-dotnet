// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Simulation run to create
    /// </summary>
    public class SimulationRunCreate
    {
        /// <summary>
        /// The simulator name
        /// </summary>
        public string SimulatorName { get; set; }

        /// <summary>
        /// The routine name
        /// </summary>
        public string RoutineName { get; set; }

        /// <summary>
        /// The model name
        /// </summary>
        public string ModelName { get; set; }
    }
}
