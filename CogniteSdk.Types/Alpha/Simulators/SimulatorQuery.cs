// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulation run query class.
    /// </summary>
    public class SimulatorQuery
    {
        /// <summary>
        /// Filter on simulators with strict matching.
        /// </summary>
        public SimulatorFilter Filter { get; set; }

    }

    /// <summary>
    /// The simulation run filter class.
    /// </summary>
    public class SimulatorFilter
    {
        /// <summary>
        /// Whether simulator is enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}
