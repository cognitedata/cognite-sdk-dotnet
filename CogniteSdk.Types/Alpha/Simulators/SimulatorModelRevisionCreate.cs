// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A Simulator model revision to create.
    /// </summary>
    public class SimulatorModelRevisionCreate
    {
        /// <summary>
        /// External id of the simulation model revision.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// External id of the simulation model.
        /// </summary>
        public string ModelExternalId { get; set; }

        /// <summary>
        /// Description of the simulation model.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Source file id of the simulation model.
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// Boundary conditions of the simulation model with target timeseries where the values are saved.
        /// </summary>
        public IEnumerable<SimulatorModelBoundaryCondition> BoundaryConditions { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionCreate>(this);
    }
}
