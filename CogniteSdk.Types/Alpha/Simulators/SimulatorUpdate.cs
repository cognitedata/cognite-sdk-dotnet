// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    public class SimulatorUpdateItem : UpdateItem<SimulatorUpdate>
    {
        /// <summary>
        /// Initialize the Simulator update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public SimulatorUpdateItem(long id) : base(id)
        {
        }
    }

    public class SimulatorUpdate
    {
        public Update<string> Name { get; set; }
        public Update<IEnumerable<string>> FileExtensionTypes { get; set; }
        public Update<bool> IsBoundaryConditionsEnabled { get; set; }
        public Update<IEnumerable<SimulatorBoundaryCondition>> BoundaryConditions { get; set; }
        public Update<bool> IsCalculationsEnabled { get; set; }
        public Update<IEnumerable<SimulatorModelType>> ModelTypes { get; set; }
        public Update<bool> Enabled { get; set; }
        public Update<IEnumerable<SimulatorStepField>> StepFields { get; set; }
        public Update<SimulatorUnits> Units { get; set; }
    }
}