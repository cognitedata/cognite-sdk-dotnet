// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The conversion between a unit and its base unit.
    /// </summary>
    public class ConversionFactor
    {
        /// <summary>
        /// The multiplier to convert from the unit to the base unit.
        /// </summary>
        public double Multiplier { get; set; }

        /// <summary>
        /// The offset to convert from the unit to the base unit.
        /// </summary>
        public double Offset { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// UnitItem read class.
    /// </summary>
    public class UnitItem
    {
        /// <summary>
        /// Unique identifier of the unit.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Name of the unit, e.g. `DEG_C` for Celsius.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// More descriptive name of the unit, e.g., `degrees Celsius`.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// The symbol of the unit, e.g., `°C`.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// List of alias names for the unit, e.g., `Degree C`, `degC`, `°C`, and so on.
        /// </summary>
        public IEnumerable<string> AliasNames { get; set; }

        /// <summary>
        /// The quantity of the unit, e.g., `Temperature`.
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// The conversion between the unit and its base unit. For example, the base unit
        /// for Temperature is Kelvin, and the conversion from Celsius to Kelvin is
        /// multiplier = 1, offset = 273.15.
        /// </summary>
        public ConversionFactor Conversion { get; set; }

        /// <summary>
        /// The source of the unit, e.g., `qudt.org`.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The reference to the source of the unit, e.g., `http://qudt.org/vocab/unit/DEG_C`.
        /// </summary>
        public string SourceReference { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
