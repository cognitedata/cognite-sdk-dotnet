// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Dynamic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator resource
    /// </summary>
    public class SimulatorCreate
    {
        /// <summary>
        /// The external id of the simulator.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// File extension types supported by the simulator.
        /// </summary>
        public string[] FileExtensionTypes { get; set; }
        /// <summary>
        /// The name of the simulator. e.g. PROSPER, OLGA, etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Whether the boundary conditions are enabled.
        /// </summary>
        public bool? IsBoundaryConditionsEnabled { get; set; }
        /// <summary>
        /// Boundary conditions supported by the simulator.
        /// </summary>
        public IEnumerable<SimulatorBoundaryCondition> BoundaryConditions { get; set; }
        /// <summary>
        /// Whether the calculations are enabled.
        /// </summary>
        public bool? IsCalculationsEnabled { get; set; }
        /// <summary>
        /// Supported model types.
        /// </summary>
        public IEnumerable<SimulatorModelType> ModelTypes { get; set; }
        /// <summary>
        ///  Whether the simulator is enabled.
        /// </summary>
        public bool? Enabled { get; set; }
        /// <summary>
        /// Supported calculation steps.
        /// </summary>
        public IEnumerable<SimulatorStepField> StepFields { get; set; }
        /// <summary>
        /// Supported units by the simulator.
        /// </summary>
        public SimulatorUnits Units { get; set; }
    }
}
