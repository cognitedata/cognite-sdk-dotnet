// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulator model class.
    /// </summary>
    public class SimulatorModelQuery
    {
        /// <summary>
        /// Filter on simulators models.
        /// </summary>
        public SimulatorModelFilter Filter { get; set; }

    }

    /// <summary>
    /// The simulator model filter class.
    /// </summary>
    public class SimulatorModelFilter
    {
        /// <summary>
        /// Filter on simulator external ids.
        /// </summary>
        public IEnumerable<string> SimulatorExternalIds { get; set; }
    }
}