// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

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
        /// Simulator external id
        /// </summary>
        public string SimulatorExternalId { get; set; }

        /// <summary>
        /// Routine external id
        /// </summary>
        public string RoutineExternalId { get; set; }

        /// <summary>
        /// Routine revision external id
        /// </summary>
        public string RoutineRevisionExternalId { get; set; }

        /// <summary>
        /// Model external id
        /// </summary>
        public string ModelExternalId { get; set; }

        /// <summary>
        /// Model revision external id
        /// </summary>
        public string ModelRevisionExternalId { get; set; }

        /// <summary>
        /// The simulator integration external id
        /// </summary>
        public string SimulatorIntegrationExternalId { get; set; }

        /// <summary>
        /// The simulation run status
        /// </summary>
        public SimulationRunStatus Status { get; set; }

        /// <summary>
        /// The simulation run type
        /// </summary>
        public SimulationRunType RunType { get; set; }

        /// <summary>
        /// The simulation run status message
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Simulation event id
        /// </summary>
        public long? EventId { get; set; }

        /// <summary>
        /// Timestamp that overwrites the timestamp at which the simulation run reads from the input time series.
        /// </summary>
        public long? RunTime { get; set; }

        /// <summary>
        /// Simulation time in milliseconds. This is the timestamp used for indexing inputs and outputs of the simulation.
        /// </summary>
        public long? SimulationTime { get; set; }

        /// <summary>
        /// Log id of the simulation run
        /// </summary>
        public long? LogId { get; set; }

        /// <summary>
        /// Time when this simulation run was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Time when this simulation run was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRun>(this);
    }
}
