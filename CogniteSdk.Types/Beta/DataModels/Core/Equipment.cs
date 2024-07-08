// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Represents a physical piece of equipment.
    /// </summary>
    public class Equipment : CoreInstanceBase, IObject3D
    {
        /// <summary>
        /// Serial number of the equipment.
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// Manufacturer of the equipment.
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// Type of this equipment, direct relation to EquipmentType.
        /// </summary>
        /// <value></value>
        public DirectRelationIdentifier EquipmentType { get; set; }
    }

    /// <summary>
    /// This identifies the type of an equipment.
    /// </summary>
    public class EquipmentType : IDescribable
    {
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Aliases { get; set; }

        /// <summary>
        /// A unique identifier for the type of equipment.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Class of equipment.
        /// </summary>
        public string EquipmentClass { get; set; }
        /// <summary>
        /// Source of the equipment specification.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Reference to the source of the equipment specification.
        /// </summary>
        public string SourceReference { get; set; }
    }
}