// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Simulation run id
    /// </summary>
    public class SimulationRunId
    {
        /// <summary>
        /// Simulation run id
        /// </summary>
        public long RunId { get; set; }

        /// <summary>
        /// Create a new SimulationRunId
        /// </summary>
        /// <param name="id">Internal id value</param>
        public static SimulationRunId Create(long id)
        {
            return new SimulationRunId()
            {
                RunId = id
            };
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRunId>(this);
    }
}
