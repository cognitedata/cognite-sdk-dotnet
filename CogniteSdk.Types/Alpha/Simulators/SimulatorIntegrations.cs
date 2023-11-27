// Run Simulation = SimulationRun
// SimulationRunCreate = 

// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Filter Simulator Integrations
    /// </summary>
    public class SimulatorIntegrations
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        public string SimulatorExternalId { get; set; }
        public long Heartbeat { get; set; }
        public long DataSetId { get; set; }
        public string ConnectorVersion { get; set; }
        public string SimulatorVersion { get; set; }
        public bool RunApiEnabled { get; set; }
        public string LicenseStatus { get; set; }
        public long LicenseLastCheckedTime { get; set; }
        public string ConnectorStatus { get; set; }
        public long ConnectorStatusUpdatedTime { get; set; }
        public long CreatedTime { get; set; }
        public long LastUpdatedTime { get; set; }
    }
}