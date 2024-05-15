// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <inheritdoc />
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

    /// <summary>
    /// A simulator update class.
    /// </summary>
    public class SimulatorUpdate
    {
        /// <summary>
        /// Update the name of the simulator.
        /// </summary>
        public Update<string> Name { get; set; }
        /// <summary>
        /// Update the file extension types supported by the simulator.
        /// </summary>
        public Update<IEnumerable<string>> FileExtensionTypes { get; set; }
        /// <summary>
        /// Update whether the boundary conditions are enabled.
        /// </summary>
        public Update<bool> IsBoundaryConditionsEnabled { get; set; }
        /// <summary>
        /// Update the boundary conditions supported by the simulator.
        /// </summary>
        public Update<IEnumerable<SimulatorBoundaryCondition>> BoundaryConditions { get; set; }
        /// <summary>
        /// Update whether the calculations are enabled.
        /// </summary>
        public Update<bool> IsCalculationsEnabled { get; set; }
        /// <summary>
        /// Update the supported model types.
        /// </summary>
        public Update<IEnumerable<SimulatorModelType>> ModelTypes { get; set; }
        /// <summary>
        /// Update whether the simulator is enabled.
        /// </summary>
        public Update<bool> Enabled { get; set; }
        /// <summary>
        /// Update the supported calculation steps.
        /// </summary>
        public Update<IEnumerable<SimulatorStepField>> StepFields { get; set; }
        /// <summary>
        /// Update the supported unit quantities and units.
        /// </summary>
        public Update<IEnumerable<SimulatorUnitQuantity>> UnitQuantities { get; set; }
    }
}
