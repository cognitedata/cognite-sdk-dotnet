// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Dynamic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateSimulators
    {
        public long ExternalId { get; set; }
        public string[] FileExtensionTypes { get; set; }
        public string Name { get; set; }
        public bool? IsBoundaryConditionsEnabled { get; set; }
        public IEnumerable<BoundaryConditions> BoundaryConditions { get; set; }
        public bool? IsCalculationsEnabled { get; set; }
        public IEnumerable<ModelTypes> ModelTypes { get; set; }
        public bool? Enabled { get; set; }
        public IEnumerable<StepFields> StepFields { get; set; }
        public Units Units { get; set; }
    }
    public class Units
    {
        public Dictionary<string, SimulatorUnitsMap> UnitsMap { get; set; }
        public Dictionary<string, SimulatorUnitSystem> UnitSystem { get; set; }
    }
    public class SimulatorUnitSystem
    {
        public string Label { get; set; }
        public  Dictionary<string, string>  DefaultUnits { get; set; }
    }
    public class SimulatorUnitsMap 
    {
        public string Label { get; set; }
        public Units2 Units { get; set; }
    }
    public class Units2
    {
        public string Label { get; set; }
        public string Value { get; set; }

    }
    public class StepFields 
    {
        public string StepType { get; set; }
        public IEnumerable<Fields> Fields { get; set; }
    }
    public class Fields 
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Info { get; set; }
        public IEnumerable<Options> Options { get; set; }
    }
    public class Options 
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
    public class ModelTypes 
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
    public class BoundaryConditions 
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Key { get; set; }
    }
}
