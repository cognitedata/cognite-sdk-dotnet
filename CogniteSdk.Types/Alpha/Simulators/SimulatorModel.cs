// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A Simulator model resource.
    /// </summary>
    public class SimulatorModel {
        /// <summary>
        /// A unique id of a simulation model.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// External id of the simulation model.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// External id of the simulator.
        /// </summary>
        public string SimulatorExternalId { get; set; }

        /// <summary>
        /// Name of the simulation model.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the simulation model.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data set id of the simulation model.
        /// </summary>
        public long DataSetId { get; set; }

        /// <summary>
        /// Labels of the simulation model.
        /// </summary>
        public IEnumerable<CogniteExternalId> Labels { get; set; }

        /// <summary>
        /// Model type of the simulation model. List of available types is available in the simulator resource.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Unit system of the simulation model.
        /// </summary>
        public string UnitSystem { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModel>(this);
    }
}
