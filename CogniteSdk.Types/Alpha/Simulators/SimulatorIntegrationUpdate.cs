// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    public class SimulatorIntegrationUpdate
    {
        public Update<long> Heartbeat { get; set; }
        public Update<long> DataSetId { get; set; }
        public Update<string> ConnectorVersion { get; set; }
        public Update<string> SimulatorVersion { get; set; }
        public Update<bool> RunApiEnabled { get; set; }
        public Update<string> LicenseStatus { get; set; }
        public Update<long> LicenseLastCheckedTime { get; set; }
        public Update<string> ConnectorStatus { get; set; }
        public Update<long> ConnectorStatusUpdatedTime { get; set; }
    }

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