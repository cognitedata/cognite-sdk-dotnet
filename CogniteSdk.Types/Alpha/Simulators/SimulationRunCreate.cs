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
        /// The simulator name, only used if RoutineExternalId is not set
        /// </summary>
        public string SimulatorName { get; set; }

        /// <summary>
        /// The routine name, only used if RoutineExternalId is not set
        /// </summary>
        public string RoutineName { get; set; }

        /// <summary>
        /// The model name, only used if RoutineExternalId is not set
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Routine external id, only used if RoutineName, SimulatorName, ModelName is not set
        /// </summary>
        public string RoutineExternalId { get; set; }

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
