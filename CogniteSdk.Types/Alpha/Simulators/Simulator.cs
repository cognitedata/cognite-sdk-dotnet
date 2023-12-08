// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Dynamic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// 
    /// </summary>
    public class Simulator
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

     public class SimulatorUnits
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
        public UnitsMapItem Units { get; set; }
    }
    public class UnitsMapItem
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
    public class SimulatorModelTypes 
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
    public class SimulatorBoundaryConditions 
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Key { get; set; }
    }
}
