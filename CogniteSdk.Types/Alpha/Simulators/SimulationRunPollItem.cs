// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Request item for polling simulation runs.
    /// </summary>
    public class SimulationRunPollItem
    {
        /// <summary>
        /// The simulator integration external id.
        /// </summary>
        public string SimulatorIntegrationExternalId { get; set; }

        /// <summary>
        /// Maximum number of runs to claim. Defaults to 10.
        /// </summary>
        public int? Limit { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulationRunPollItem>(this);
    }
}
