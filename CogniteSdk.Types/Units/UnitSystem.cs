// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Quantity read class.
    /// </summary>
    public class Quantity
    {
        /// <summary>
        /// Quantity name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// UnitItem external id associated with the quantity.
        /// </summary>
        public string UnitExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// UnitItem system read class.
    /// </summary>
    public class UnitSystem
    {
        /// <summary>
        /// Name of the unit system.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of quantities associated with the unit system.
        /// </summary>
        public IEnumerable<Quantity> Quantities { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}