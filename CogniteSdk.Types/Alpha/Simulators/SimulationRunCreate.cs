// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// Simulation run input unit override
    /// </summary>
    public class SimulationInputUnitOverride
    {
        /// <summary>
        /// The unit name
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Stringable.ToString<SimulationInputUnitOverride>(this);
    }

    /// <summary>
    /// Simulation run type
    /// </summary>
    public class SimulationInputOverride
    {
        /// <summary>
        /// The original input reference id from the routine configuration
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// The input value
        /// </summary>
        public SimulatorValue Value { get; set; }

        /// <summary>
        /// The input unit
        /// </summary>
        public SimulationInputUnitOverride Unit { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationInputOverride>(this);
    }
    

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
        /// The simulation inputs to override the default routine input values
        /// </summary>
        public IEnumerable<SimulationInputOverride> Inputs { get; set; }

        /// <summary>
        /// Run type
        /// </summary>
        public SimulationRunType RunType { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRunCreate>(this);
    }
}
