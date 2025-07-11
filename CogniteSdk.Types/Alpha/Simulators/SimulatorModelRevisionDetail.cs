// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A Simulator model revision resource with detailed information including external dependencies.
    /// </summary>
    public class SimulatorModelRevisionDetail : SimulatorModelRevision
    {
        /// <summary>
        /// List of external dependencies of the simulation model revision.
        /// Only used for the simulators that support models consisting of multiple files.
        /// </summary>
        public IEnumerable<SimulatorFileDependency> ExternalDependencies { get; set; }
    }
}
