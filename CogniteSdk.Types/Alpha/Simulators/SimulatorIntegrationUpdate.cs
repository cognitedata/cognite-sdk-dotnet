// Run Simulation = SimulationRun
// SimulationRunCreate = 

// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Filter Simulator Integrations
    /// </summary>
    public class SimulatorIntegrationUpdate
    {
        public long Id { get; set; }
        public SimulatorIntegrationUpdateUpdate Update { get; set; }
    }

    public class SimulatorIntegrationUpdateUpdate
    {
        public Update<long> Heartbeat { get; set; }
        public Update<long> DataSetId { get; set; }
        public Update<object> ConnectorVersion { get; set; }
        public Update<string> SimulatorVersion { get; set; }
        public Update<bool> RunApiEnabled { get; set; }
        public Update<string> LicenseStatus { get; set; }
        public Update<long?> LicenseStatusLastCheckedTime { get; set; }
        public Update<string> ConnectorStatus { get; set; }
        public Update<long?> ConnectorStatusUpdatedTime { get; set; }
    }
}