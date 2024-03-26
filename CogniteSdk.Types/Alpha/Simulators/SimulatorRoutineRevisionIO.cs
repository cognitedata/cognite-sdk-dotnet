// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json.Serialization;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Simulator routine revision input.
    /// </summary>
    public class SimulatorRoutineRevisionInput {
        /// <summary>
        /// Name of the input.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference id of the input.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// If defined, this is time series external ID the value will be read from.
        /// </summary>
        public string SourceExternalId { get; set; }

        /// <summary>
        /// If defined, this is time series aggregation to use when reading the value.
        /// Allowed values are: "average", "interpolation", "stepInterpolation".
        /// </summary>
        public string Aggregate { get; set; }

        /// <summary>
        /// Value of the input. Only used if the input is not a time series.
        /// </summary>
        public SimulatorValue Value { get; set; }

        /// <summary>
        /// Value type of the input.
        /// </summary>
        public SimulatorValueType ValueType { get; set; }

        /// <summary>
        /// Unit of the input.
        /// </summary>
        public SimulatorValueUnit Unit { get; set; }

        /// <summary>
        /// If defined, this is time series external ID the value will be persisted to.
        /// </summary>
        public string SaveTimeseriesExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);

        /// <summary>
        /// Whether the input is a time series.
        /// </summary>
        [JsonIgnore]
        public bool IsTimeSeries => !String.IsNullOrEmpty(SourceExternalId);

        /// <summary>
        /// Whether the input is a constant value.
        /// </summary>
        [JsonIgnore]
        public bool IsConstant => Value != null;
    }

    /// <summary>
    /// Simulator routine revision output.
    /// </summary>
    public class SimulatorRoutineRevisionOutput {
        /// <summary>
        /// Name of the output.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference id of the output.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Value type of the output.
        /// </summary>
        public SimulatorValueType ValueType { get; set; }

        /// <summary>
        /// Unit of the output.
        /// </summary>
        public SimulatorValueUnit Unit { get; set; }

        /// <summary>
        /// If defined, this is time series external ID the value will be persisted to.
        /// </summary>
        public string SaveTimeseriesExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
