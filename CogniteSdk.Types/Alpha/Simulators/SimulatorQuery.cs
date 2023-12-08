// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Text.Json.Serialization;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulation run query class.
    /// </summary>
    public class SimulatorsQuery
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