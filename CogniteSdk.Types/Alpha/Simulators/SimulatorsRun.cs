// Run Simulation = SimulationRun
// SimulationRunCreate = 

// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Filter Simulator Integrations
    /// </summary>
    public class SimulatorsRun
    {
        public long Id { get; set; }
        public long ExternalId { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> FileExtensionTypes { get; set; }
        public bool IsBoundaryCondiationsEnabled { get; set; }
        public BoundaryConditions BoundaryConditions { get; set; }
        public bool IsCalculationsEnabled { get; set; }
        public ModelTypes ModelTypes { get; set; }
        public bool Enabled { get; set; }
        public StepFields StepFields { get; set; }
        public Units Units { get; set; }
        public long CreatedTime { get; set; }
        public long LastUpdatedTime { get; set; }

    }
}