// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;
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
    /// Type of a simulation run
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SimulationRunType
    {
        /// <summary>
        /// external
        /// </summary>
        external,
        /// <summary>
        /// manual
        /// </summary>
        manual,
        /// <summary>
        /// scheduled
        /// </summary>
        scheduled,
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

        /// <summary>
        /// Sort order.
        /// </summary>
        public IEnumerable<SimulatorSortItem> Sort { get; set; }

        /// <summary>
        /// Limits the number of results to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRunQuery>(this);
    }

    /// <summary>
    /// The simulation run filter class.
    /// </summary>
    public class SimulationRunFilter
    {
        /// <summary>
        /// Filter by simulator external ids
        /// </summary>
        public IEnumerable<string> SimulatorExternalIds { get; set; }

        /// <summary>
        /// Filter by simulator integration external ids
        /// </summary>
        public IEnumerable<string> SimulatorIntegrationExternalIds { get; set; }

        /// <summary>
        /// Filter by model revision external ids
        /// </summary>
        public IEnumerable<string> ModelRevisionExternalIds { get; set; }

        /// <summary>
        /// Filter by routine revision external ids
        /// </summary>
        public IEnumerable<string> RoutineRevisionExternalIds { get; set; }

        /// <summary>
        /// Simulator integration external id
        /// </summary>
        public string SimulatorIntegrationExternalId { get; set; }

        /// <summary>
        /// The simulation run status
        /// </summary>
        public SimulationRunStatus? Status { get; set; }

        /// <summary>
        /// Filter by simulation run time
        /// </summary>
        public TimeRange SimulationTime { get; set; }

        /// <summary>
        /// Filter by created time
        /// </summary>
        public TimeRange CreatedTime { get; set; }
    }
}