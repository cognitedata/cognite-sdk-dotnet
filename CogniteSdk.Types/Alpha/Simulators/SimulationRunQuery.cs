// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Text.Json.Serialization;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Status of a simulation run
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SimulationRunStatus
    {
        /// <summary>
        /// ready
        /// </summary>
        ready,
        /// <summary>
        /// running
        /// </summary>
        running,
        /// <summary>
        /// success
        /// </summary>
        success,
        /// <summary>
        /// failure
        /// </summary>
        failure,
    }

    /// <summary>
    /// The Simulation run query class.
    /// </summary>
    public class SimulationRunQuery
    {
        /// <summary>
        /// Filter on assets with strict matching.
        /// </summary>
        public SimulationRunFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRunQuery>(this);
    }

    /// <summary>
    /// The simulation run filter class.
    /// </summary>
    public class SimulationRunFilter
    {
        /// <summary>
        /// The simulator name
        /// </summary>
        public string SimulatorName { get; set; }

        /// <summary>
        /// The routine name
        /// </summary>
        public string RoutineName { get; set; }

        /// <summary>
        /// The model name
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// The simulation run status
        /// </summary>
        public SimulationRunStatus? Status { get; set; }
    }
}