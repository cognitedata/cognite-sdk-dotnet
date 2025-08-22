// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

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
        public IEnumerable<string> FileExtensionTypes { get; set; }
        /// <summary>
        /// Supported model types.
        /// </summary>
        public IEnumerable<SimulatorModelType> ModelTypes { get; set; }
        /// <summary>
        /// List of model dependencies supported by the simulator, includes the allowed fields and file extensions.
        /// </summary>
        public IEnumerable<SimulatorModelDependency> ModelDependencies { get; set; }
        /// <summary>
        /// Supported calculation steps.
        /// </summary>
        public IEnumerable<SimulatorStepField> StepFields { get; set; }
        /// <summary>
        /// Supported unit quantities and units.
        /// </summary>
        public IEnumerable<SimulatorUnitQuantity> UnitQuantities { get; set; }
        /// <summary>
        /// Created time in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Updated time in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Represents the mapping of units to quantity of measurement.
    /// </summary>
    public class SimulatorUnitQuantity
    {
        /// <summary>
        /// The label of the quantity of measurement. For display purposes.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The name of the quantity of measurement.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The collection of units for the quantity of measurement.
        /// </summary>
        public IEnumerable<SimulatorUnitEntry> Units { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Represents an item in the units map.
    /// </summary>
    public class SimulatorUnitEntry
    {
        /// <summary>
        /// The label of the unit, for display purposes. E.g. meter, kilometer, etc.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The name of the unit. E.g. m, ft, etc.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
