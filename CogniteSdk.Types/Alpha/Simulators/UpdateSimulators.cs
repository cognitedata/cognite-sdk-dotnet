// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Dynamic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Simulation run to create
    /// </summary>
    public class UpdateSimulators
    {
        public int Id { get; set; }
        public SimulatorUpdate Update { get; set; }
    }

    public class SimulatorUpdate // All optional and all sets
    { // Unsure about this one TODO:
        public Update<IEnumerable<string>> Name { get; set; }
        public Update<IEnumerable<string>> FileExtensionTypes { get; set; }
        public Update<bool> IsBoundaryConditionsEnabled { get; set; } //double check this one
        public Update<IEnumerable<SimulatorBoundaryCondition>> BoundaryConditions { get; set; }
        public Update<bool> IsCalculationsEnabled { get; set; }
        public Update<IEnumerable<SimulatorModelType>> ModelTypes { get; set; }
        public Update<bool> Enabled { get; set; }
        public Update<IEnumerable<SimulatorStep>> StepFields { get; set; } // Double check that you didnt write anything twice
        public Update<Units> Units { get; set; }
    }

    public class SimulatorStep
    {
        public string StepType { get; set; }
        public SimulatorStepFields Fields { get; set; }
    }
    public class SimulatorStepFields
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Info { get; set; }
        public IEnumerable<SimulatorStepOption> Options { get; set; }
    }
    public class SimulatorStepOption
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class SimulatorModelType
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class SimulatorBoundaryCondition
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Key { get; set; }
    }
}