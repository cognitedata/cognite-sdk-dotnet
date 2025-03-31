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
        /// Flowsheet of the model revision.
        /// </summary>
        public Update<SimulatorModelRevisionDataFlowsheet> Flowsheet { get; set; }

        /// <summary>
        /// Additional simulator-specific information.
        /// </summary>
        public Update<Dictionary<string, string>> Info { get; set; }

    }

    /// <summary>
    /// Update class by model revision external id.
    /// </summary>
    public class UpdateItemWithModelRevisionExternalId<TUpdate> : UpdateItem<TUpdate>
    {
        /// <summary>
        /// Initialize the update item with a UUID.
        /// </summary>
        /// <param name="modelRevisionExternalId">External id of the model revision.</param>
        public UpdateItemWithModelRevisionExternalId(string modelRevisionExternalId) : base(modelRevisionExternalId)
        {
        }
    }

    /// <summary>
    /// Update class for simulator model revision data.xx
    /// </summary>

    public class SimulatorModelRevisionDataUpdateItem : UpdateItemWithModelRevisionExternalId<SimulatorModelRevisionDataUpdate>
    {
        /// <summary>
        /// External id of the model revision.
        /// </summary>
        public SimulatorModelRevisionDataUpdateItem(string ModelRevisionExternalId) : base(ModelRevisionExternalId)
        {
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionDataUpdateItem>(this);
    }

}
