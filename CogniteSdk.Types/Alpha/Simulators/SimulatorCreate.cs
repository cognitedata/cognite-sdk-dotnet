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
        public long ExternalId { get; set; }
        public string[] FileExtensionTypes { get; set; }
        public string Name { get; set; }
        public bool? IsBoundaryConditionsEnabled { get; set; }
        public IEnumerable<SimulatorBoundaryConditions> BoundaryConditions { get; set; }
        public bool? IsCalculationsEnabled { get; set; }
        public IEnumerable<SimulatorModelTypes> ModelTypes { get; set; }
        public bool? Enabled { get; set; }
        public IEnumerable<StepFields> StepFields { get; set; }
        public SimulatorUnits Units { get; set; }
    }
}
