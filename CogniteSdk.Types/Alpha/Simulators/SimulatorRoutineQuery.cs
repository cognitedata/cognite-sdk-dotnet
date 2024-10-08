// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulation Routine query class.
    /// </summary>
    public class SimulatorRoutineQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on simulators with strict matching.
        /// </summary>
        public SimulatorRoutineFilter Filter { get; set; }

        /// <summary>
        /// Sort order.
        /// </summary>
        public IEnumerable<SimulatorSortItem> Sort { get; set; }

    }

    /// <summary>
    /// The simulation routine filter class.
    /// </summary>
    public class SimulatorRoutineFilter
    {
        /// <summary>
        /// List of model ExternalIds to match.
        /// </summary>
        public IEnumerable<string> ModelExternalIds { get; set; }

        /// <summary>
        /// List of simulator integration ExternalIds to match.
        /// </summary>
        public IEnumerable<string> SimulatorIntegrationExternalIds { get; set; }
    }
}
