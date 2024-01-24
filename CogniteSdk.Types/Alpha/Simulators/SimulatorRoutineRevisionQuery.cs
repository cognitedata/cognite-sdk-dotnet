// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulation Routine Revision query class.
    /// </summary>
    public class SimulatorRoutineRevisionQuery
    {
        /// <summary>
        /// Filter on simulators with strict matching.
        /// </summary>
        public SimulatorRoutineRevisionFilter Filter { get; set; }

        /// <summary>
        /// Limit the number of results.
        /// </summary>
        /// <value>Default to 100. Max is 1000.</value>
        public int? Limit { get; set; }

        /// <summary>
        /// Sort order.
        /// </summary>
        public IEnumerable<SimulatorSortItem> Sort { get; set; }

    }

    /// <summary>
    /// The simulation routine revision filter class.
    /// </summary>
    public class SimulatorRoutineRevisionFilter
    {
        /// <summary>
        /// List of routineExternalIds strings
        /// </summary>
        public List<string> RoutineExternalIds { get; set; }

    }
}
