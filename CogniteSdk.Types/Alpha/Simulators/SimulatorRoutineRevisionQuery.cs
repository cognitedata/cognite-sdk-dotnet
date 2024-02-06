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
        public IEnumerable<string> RoutineExternalIds { get; set; }

        /// <summary>
        /// Filter by simulator external Ids
        /// </summary>
        public IEnumerable<string> SimulatorExternalIds { get; set; }

        /// <summary>
        /// Filter by model external Ids
        /// </summary>
        public IEnumerable<string> ModelExternalIds { get; set; }

        /// <summary>
        /// Filter by created time
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Filter by simulator integration external Ids
        /// </summary>
        public IEnumerable<string> SimulatorIntegrationExternalIds { get; set; }

    }
}
