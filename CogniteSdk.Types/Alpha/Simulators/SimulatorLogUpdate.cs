// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator log update class.
    /// Used by the connector to update the log entries.
    /// </summary>
    public class SimulatorLogUpdate
    {
        /// <summary>
        /// List of log entries.
        /// </summary>
        public UpdateEnumerable<SimulatorLogDataEntry> Data { get; set; }
    }

    /// <inheritdoc/>
    public class SimulatorLogUpdateItem : UpdateItem<SimulatorLogUpdate>
    {
        /// <summary>
        /// Initialize the simulator log update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public SimulatorLogUpdateItem(long id) : base(id)
        {
        }
    }
}
