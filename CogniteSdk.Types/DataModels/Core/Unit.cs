// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.DataModels.Core
{
    /// <summary>
    /// Core data model representation of an unit of measurement.
    /// </summary>
    public class CogniteUnit : CogniteDescribable
    {
        /// <summary>
        /// The symbol for the unit of measurement.
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// Specifies the physical quantity the unit measures.
        /// </summary>
        public string Quantity { get; set; }
        /// <summary>
        /// Source of the unit specification.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Reference to the source of the unit specification. This should preferably be a valid URL to a standard or documentation on the source.
        /// </summary>
        public string SourceReference { get; set; }

        /// <summary>
        /// List of UnitSystems referencing this unit.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> UnitSystems { get; set; }
    }

    /// <summary>
    /// Core data model representation of an unit of measurement.
    /// </summary>
    public class CogniteUnitSystem : CogniteDescribable
    {
        /// <summary>
        /// List of direct relations to all unit instances in this unit system.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Quantities { get; set; }
    }
}
