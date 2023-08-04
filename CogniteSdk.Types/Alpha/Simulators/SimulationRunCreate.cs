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

        /// <summary>
        /// Timestamp that overwrites the validation end
        /// </summary>
        public long? ValidationEndTime { get; set; }

        /// <summary>
        /// Queue the simulation run when connector is down
        /// </summary>
        public bool Queue { get; set; }

        /// <summary>
        /// Run type
        /// </summary>
        public SimulationRunType RunType { get; set; }
    }
}
