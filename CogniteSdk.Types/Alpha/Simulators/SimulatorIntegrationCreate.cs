// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator integration create class.
    /// Represents a single simulator connector in CDF.
    /// </summary>
    public class SimulatorIntegrationCreate
    {
        /// <summary>
        /// The external id of the simulator integration.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// External id of the simulator.
        /// </summary>
        public string SimulatorExternalId { get; set; }
        /// <summary>
        /// The timestamp of the last heartbeat in milliseconds since Jan 1, 1970.
        /// </summary>
        public long Heartbeat { get; set; }
        /// <summary>
        /// The id of the dataset.
        /// </summary>
        public long DataSetId { get; set; }
        /// <summary>
        /// The version of the connector.
        /// </summary>
        public string ConnectorVersion { get; set; }
        /// <summary>
        /// The version of the simulator.
        /// </summary>
        public string SimulatorVersion { get; set; }
        /// <summary>
        /// Whether the simulator runs api is enabled.
        /// </summary>
        public bool RunApiEnabled { get; set; }
        /// <summary>
        /// The status of the license.
        /// </summary>
        public string LicenseStatus { get; set; }
        /// <summary>
        /// The timestamp of the last license check in milliseconds since Jan 1, 1970.
        /// </summary>
        public long? LicenseLastCheckedTime { get; set; }
        /// <summary>
        /// The status of the connector.
        /// </summary>
        public string ConnectorStatus { get; set; }
        /// <summary>
        /// The timestamp of the last connector status update in milliseconds since Jan 1, 1970.
        /// </summary>
        public long? ConnectorStatusUpdatedTime { get; set; }
    }
}