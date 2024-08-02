// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator model revision update class.
    /// Used by the simulator connector.
    /// </summary>
    public class SimulatorModelRevisionUpdate
    {
        /// <summary>
        /// Update the status of the simulation model revision.
        /// </summary>
        public Update<SimulatorModelRevisionStatus> Status { get; set; }

        /// <summary>
        /// Update the status message of the simulation model revision.
        /// </summary>
        public Update<string> StatusMessage { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionUpdate>(this);
    }

    /// <inheritdoc/>
    public class SimulatorModelRevisionUpdateItem : UpdateItem<SimulatorModelRevisionUpdate>
    {
        /// <summary>
        /// Initialize the simulator model revision update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public SimulatorModelRevisionUpdateItem(long id) : base(id)
        {
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionUpdateItem>(this);
    }
}
