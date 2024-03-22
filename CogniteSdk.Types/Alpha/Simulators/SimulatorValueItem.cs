// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// Simulation run input/output value unit
    /// </summary>
    public class SimulatorValueUnit
    {
        /// <summary>
        /// The input name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unit type
        /// </summary>
        public string Type { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Stringable.ToString<SimulatorValueUnit>(this);
    }

    /// <summary>
    /// Simulator value item (input or output)
    /// </summary>
    public class SimulatorValueItem
    {
        /// <summary>
        /// The original input reference id from the routine configuration
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// The input value
        /// </summary>
        public SimulatorValue Value { get; set; }

        /// <summary>
        /// The input value type
        /// </summary>
        public SimulatorValueType ValueType { get; set; }

        /// <summary>
        /// Whether the input was overridden for this simulation run
        /// </summary>
        public bool? Overridden { get; set; }

        /// <summary>
        /// This is where the value was written to (for inputs) or read from (for outputs)
        /// </summary>
        public Dictionary<string, string> SimulatorObjectReference { get; set; }


        /// <summary>
        /// The input unit
        /// </summary>
        public SimulatorValueUnit Unit { get; set; }

        /// <summary>
        /// Optional, only used if the value is persisted as time series
        /// </summary>
        public string TimeseriesExternalId { get; set; }
        
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorValueItem>(this);
    }
}
