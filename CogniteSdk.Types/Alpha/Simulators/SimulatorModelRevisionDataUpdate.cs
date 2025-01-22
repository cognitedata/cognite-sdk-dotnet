// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Update class for simulator model revision data.
    /// </summary>
    public class SimulatorModelRevisionDataUpdate
    {
        /// <summary>
        /// Flowsheet of the model revision.
        /// </summary>
        public Update<SimulatorModelRevisionDataFlowsheet> Flowsheet { get; set; }

        /// <summary>
        /// Additional simulator-specific information.
        /// </summary>
        public Update<Dictionary<string, string>> Info { get; set; }
    }
}
