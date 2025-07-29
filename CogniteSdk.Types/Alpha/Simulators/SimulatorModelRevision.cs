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
    public enum SimulatorModelRevisionStatus
    {
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
    /// A Simulator model revision resource.
    /// </summary>
    public class SimulatorModelRevision
    {
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
        /// Version number, unique per simulation model.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Log id of the model revision
        /// </summary>
        public long LogId { get; set; }

        /// <summary>
        /// List of external dependencies of the simulation model revision. Only used for the simulators that support models consisting of multiple files.
        /// </summary>
        public IEnumerable<SimulatorFileDependency> ExternalDependencies { get; set; }

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
