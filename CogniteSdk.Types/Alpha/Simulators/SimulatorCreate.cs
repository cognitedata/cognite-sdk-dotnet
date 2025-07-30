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
        public IEnumerable<string> FileExtensionTypes { get; set; }
        /// <summary>
        /// The name of the simulator. e.g. PROSPER, OLGA, etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of model dependencies supported by the simulator, includes the allowed fields and file extensions.
        /// </summary>
        public IEnumerable<SimulatorModelDependency> ModelDependencies { get; set; }
        /// <summary>
        /// Supported model types.
        /// </summary>
        public IEnumerable<SimulatorModelType> ModelTypes { get; set; }
        /// <summary>
        /// Supported calculation steps.
        /// </summary>
        public IEnumerable<SimulatorStepField> StepFields { get; set; }
        /// <summary>
        /// Supported unit quantities and units.
        /// </summary>
        public IEnumerable<SimulatorUnitQuantity> UnitQuantities { get; set; }
    }
}
