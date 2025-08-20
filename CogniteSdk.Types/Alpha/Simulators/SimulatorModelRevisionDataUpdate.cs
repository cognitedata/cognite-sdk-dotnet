// Copyright 2025 Cognite AS
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
        /// IEnumerable of simulator model revision data flowsheets.
        /// </summary>
        public Update<IEnumerable<SimulatorModelRevisionDataFlowsheet>> Flowsheets { get; set; }

        /// <summary>
        /// Additional simulator-specific information.
        /// </summary>
        public Update<Dictionary<string, string>> Info { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionDataUpdate>(this);
    }

    /// <summary>
    /// Update class for simulator model revision data.
    /// </summary>

    public class SimulatorModelRevisionDataUpdateItem

    {
        /// <summary>
        /// External id of the model revision.
        /// </summary>
        public string ModelRevisionExternalId { get; set; }

        /// <summary>
        /// Update object for the model revision data.
        /// </summary>
        public SimulatorModelRevisionDataUpdate Update { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionDataUpdateItem>(this);
    }

}
