// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulator model revision class.
    /// </summary>
    public class SimulatorModelRevisionQuery
    {
        /// <summary>
        /// Filter on simulators model revisions.
        /// </summary>
        public SimulatorModelRevisionFilter Filter { get; set; }

    }

    /// <summary>
    /// The simulator model revision filter class.
    /// </summary>
    public class SimulatorModelRevisionFilter
    {
        /// <summary>
        /// Filter on model external ids.
        /// </summary>
        public IEnumerable<string> ModelExternalIds { get; set; }
    }
}