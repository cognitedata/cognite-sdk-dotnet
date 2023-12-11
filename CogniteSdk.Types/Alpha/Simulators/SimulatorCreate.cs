// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Dynamic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// 
    /// </summary>
    public class SimulatorCreate
    {
        public string ExternalId { get; set; }
        public string[] FileExtensionTypes { get; set; }
        public string Name { get; set; }
        public bool? IsBoundaryConditionsEnabled { get; set; }
        public IEnumerable<SimulatorBoundaryCondition> BoundaryConditions { get; set; }
        public bool? IsCalculationsEnabled { get; set; }
        public IEnumerable<SimulatorModelType> ModelTypes { get; set; }
        public bool? Enabled { get; set; }
        public IEnumerable<SimulatorStepField> StepFields { get; set; }
        public SimulatorUnits Units { get; set; }
    }
}
