// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Simulation run
    /// </summary>
    public class SimulationRun
    {
        /// <summary>
        /// Simulation run id
        /// </summary>
        public long Id { get; set; }

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
        /// The simulation run status
        /// </summary>
        public SimulationRunStatus Status { get; set; }

        /// <summary>
        /// The simulation run status message
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Simulation event id
        /// </summary>
        public long? EventId { get; set; }

        /// <summary>
        /// Time when this simulation run was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Time when this simulation run was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
