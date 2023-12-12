// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator resource
    /// </summary>
    public class Simulator
    {
        /// <summary>
        /// The id of the simulator.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The external id of the simulator.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// The name of the simulator. e.g. PROSPER, OLGA, etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// File extension types supported by the simulator.
        /// </summary>
        public string[] FileExtensionTypes { get; set; }
        /// <summary>
        /// Whether the boundary conditions are enabled.
        /// </summary>
        public bool IsBoundaryConditionsEnabled { get; set; }
        /// <summary>
        /// Boundary conditions supported by the simulator.
        /// </summary>
        public IEnumerable<SimulatorBoundaryCondition> BoundaryConditions { get; set; }
        /// <summary>
        /// Whether the calculations are enabled.
        /// </summary>
        public bool IsCalculationsEnabled { get; set; }
        /// <summary>
        /// Supported model types.
        /// </summary>
        public IEnumerable<SimulatorModelType> ModelTypes { get; set; }
        /// <summary>
        /// Whether the simulator is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Supported calculation steps.
        /// </summary>
        public IEnumerable<SimulatorStepField> StepFields { get; set; }
        /// <summary>
        /// Supported units by the simulator.
        /// </summary>
        public SimulatorUnits Units { get; set; }
        /// <summary>
        /// Created time in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Updated time in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }

    /// <summary>
    /// Represents the units used by the simulator.
    /// </summary>
    public class SimulatorUnits
    {
        /// <summary>
        /// The mapping of measurements and their supported units.
        /// </summary>
        public Dictionary<string, SimulatorUnitsMap> UnitsMap { get; set; }
        /// <summary>
        /// The map of unit systems and their default units.
        /// </summary>
        public Dictionary<string, SimulatorUnitSystem> UnitSystem { get; set; }
    }
    /// <summary>
    /// Represents the unit system used by the simulator.
    /// </summary>
    public class SimulatorUnitSystem
    {
        /// <summary>
        /// The label of the unit system.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The default units for given quantities.
        /// </summary>
        public Dictionary<string, string> DefaultUnits { get; set; }
    }
    /// <summary>
    /// Represents the mapping of units used by the simulator.
    /// </summary>
    public class SimulatorUnitsMap 
    {
        /// <summary>
        /// The label of the units map.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The collection of units in the map.
        /// </summary>
        public IEnumerable<UnitsMapItem> Units { get; set; }
    }

    /// <summary>
    /// Represents an item in the units map.
    /// </summary>
    public class UnitsMapItem
    {
        /// <summary>
        /// The label of the units map item.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The value of the units map item.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents a step field in the simulator.
    /// </summary>
    public class SimulatorStepField
    {
        /// <summary>
        /// The type of the step.
        /// </summary>
        public string StepType { get; set; }
        /// <summary>
        /// The fields of the step.
        /// </summary>
        public IEnumerable<SimulatorStepFieldParam> Fields { get; set; }
    }

    /// <summary>
    /// Represents a parameter of a step field in the simulator.
    /// </summary>
    public class SimulatorStepFieldParam
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The label of the parameter.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Additional information about the parameter.
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// The additional options for the parameter.
        /// </summary>
        public IEnumerable<SimulatorStepFieldOption> Options { get; set; }
    }
    
    /// <summary>
    /// Represents an option for a step field in the simulator.
    /// </summary>
    public class SimulatorStepFieldOption 
    {
        /// <summary>
        /// The label of the step field option.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The value of the step field option.
        /// </summary>
        public string Value { get; set; }
    }
    /// <summary>
    /// Represents a model type in the simulator.
    /// </summary>
    public class SimulatorModelType 
    {
        /// <summary>
        /// The name of the model type.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The key of the model type.
        /// </summary>
        public string Key { get; set; }
    }
    /// <summary>
    /// Represents a boundary condition in the simulator.
    /// </summary>
    public class SimulatorBoundaryCondition 
    {
        /// <summary>
        /// The name of the boundary condition.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The address of the boundary condition.
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// The key of the boundary condition.
        /// </summary>
        public string Key { get; set; }
    }
}
