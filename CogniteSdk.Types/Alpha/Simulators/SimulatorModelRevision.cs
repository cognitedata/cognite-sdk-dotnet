// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A Simulator model revision status.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SimulatorModelRevisionStatus {
        /// <summary>
        /// Unknown
        /// </summary>
        unknown,
        /// <summary>
        /// Success
        /// </summary>
        success,
        /// <summary>
        /// Failure
        /// </summary>
        failure
    }

    /// <summary>
    /// A Simulator model boundary condition.
    /// </summary>
    public class SimulatorModelBoundaryCondition {
        /// <summary>
        /// Key of the boundary condition.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Name of the boundary condition.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adress of the boundary condition.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Time series external id of the boundary condition.
        /// </summary>
        public string TimeSeriesExternalId { get; set; }
    }

    /// <summary>
    /// A Simulator model revision resource.
    /// </summary>
    public class SimulatorModelRevision {
        /// <summary>
        /// A unique id of a simulation model revision.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// External id of the simulation model revision.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// External id of the simulator.
        /// </summary>
        public string SimulatorExternalId { get; set; }

        /// <summary>
        /// External id of the simulation model.
        /// </summary>
        public string ModelExternalId { get; set; }

        /// <summary>
        /// Description of the simulation model.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data set id of the simulation model.
        /// </summary>
        public long DataSetId { get; set; }

        /// <summary>
        /// Source file id of the simulation model.
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// Author of the simulation model.
        /// </summary>
        public string CreatedByUserId { get; set; }

        /// <summary>
        /// Status of the simulation model.
        /// </summary>
        public SimulatorModelRevisionStatus Status { get; set; }

        /// <summary>
        /// Status message of the simulation model.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Boundary conditions of the simulation model with target timeseries where the values are saved.
        /// </summary>
        public IEnumerable<SimulatorModelBoundaryCondition> BoundaryConditions { get; set; }

        /// <summary>
        /// Status of the boundary conditions of the simulation model.
        /// </summary>
        public SimulatorModelRevisionStatus BoundaryConditionsStatus { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevision>(this);
    }
}