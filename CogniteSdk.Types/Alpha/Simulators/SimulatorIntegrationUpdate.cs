// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator integration update class.
    /// Used by the connector to update the status of the simulator integration on a regular basis.
    /// </summary>
    public class SimulatorIntegrationUpdate
    {
        /// <summary>
        /// Update the heartbat of the simulator integration.
        /// </summary>
        public Update<long> Heartbeat { get; set; }
        /// <summary>
        /// Update the id of the dataset.
        /// </summary>
        public Update<long> DataSetId { get; set; }
        /// <summary>
        /// Update the version of the connector.
        /// </summary>
        public Update<string> ConnectorVersion { get; set; }
        /// <summary>
        /// Update the version of the simulator.
        /// </summary>
        public Update<string> SimulatorVersion { get; set; }
        /// <summary>
        /// Update whether the simulator runs api is enabled.
        /// </summary>
        public Update<bool> RunApiEnabled { get; set; }
        /// <summary>
        /// Update the status of the license.
        /// </summary>
        public Update<string> LicenseStatus { get; set; }
        /// <summary>
        /// Update the timestamp of the last license check in milliseconds since Jan 1, 1970.
        /// </summary>
        public Update<long> LicenseLastCheckedTime { get; set; }
        /// <summary>
        /// Update the status of the connector.
        /// </summary>
        public Update<string> ConnectorStatus { get; set; }
        /// <summary>
        /// Update the timestamp of the last connector status update in milliseconds since Jan 1, 1970.
        /// </summary>
        public Update<long> ConnectorStatusUpdatedTime { get; set; }
    }

    /// <inheritdoc/>
    public class SimulatorIntegrationUpdateItem : UpdateItem<SimulatorIntegrationUpdate>
    {
        /// <summary>
        /// Initialize the simulator integration update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public SimulatorIntegrationUpdateItem(long id) : base(id)
        {
        }
    }
}
