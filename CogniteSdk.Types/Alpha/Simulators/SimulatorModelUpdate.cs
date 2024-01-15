// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A simulator model update class.
    /// </summary>
    public class SimulatorModelUpdate
    {
        /// <summary>
        /// Update the name of the simulator model.
        /// </summary>
        public Update<string> Name { get; set; }

        /// <summary>
        /// Update the description of the simulator model.
        /// </summary>
        public Update<string> Description { get; set; }

        /// <summary>
        /// Update the labels of the simulator model.
        /// </summary>
        public Update<IEnumerable<string>> Labels { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelUpdate>(this);
    }

    /// <inheritdoc/>
    public class SimulatorModelUpdateItem : UpdateItem<SimulatorModelUpdate>
    {
        /// <summary>
        /// Initialize the simulator model update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public SimulatorModelUpdateItem(long id) : base(id)
        {
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelUpdateItem>(this);
    }
}
