// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// The Simulator model class.
    /// </summary>
    public class SimulatorModelQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on simulators models.
        /// </summary>
        public SimulatorModelFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelQuery>(this);
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelFilter>(this);
    }
}
