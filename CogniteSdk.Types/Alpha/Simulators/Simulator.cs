// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// 
    /// </summary>
    public class Simulator
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string[] FileExtensionTypes { get; set; }
        public bool IsBoundaryConditionsEnabled { get; set; }
        public IEnumerable<SimulatorBoundaryCondition> BoundaryConditions { get; set; }
        public bool IsCalculationsEnabled { get; set; }
        public IEnumerable<SimulatorModelType> ModelTypes { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<SimulatorStepField> StepFields { get; set; }
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
    public class SimulatorStepField
    {
        public string StepType { get; set; }
        public IEnumerable<SimulatorStepFieldParam> Fields { get; set; }
    }
    public class SimulatorStepFieldParam 
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Info { get; set; }
        public IEnumerable<SimulatorStepFieldOption> Options { get; set; }
    }
    public class SimulatorStepFieldOption 
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
