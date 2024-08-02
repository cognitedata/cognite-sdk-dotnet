// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Represents a physical piece of equipment.
    /// </summary>
    public class CogniteEquipment : CogniteCoreInstanceBase
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

        /// <summary>
        /// Asset this equipment relates to.
        /// </summary>
        public DirectRelationIdentifier Asset { get; set; }
        /// <summary>
        /// List of activities this equipment relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Activities { get; set; }
        /// <summary>
        /// List of timeseries this equipment relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Timeseries { get; set; }
        /// <summary>
        /// List of files this equipment relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Files { get; set; }
        /// <summary>
        /// Model3D this equipment relates to.
        /// </summary>
        public DirectRelationIdentifier Model3D { get; set; }
    }

    /// <summary>
    /// This identifies the type of an equipment.
    /// </summary>
    public class CogniteEquipmentType : CogniteDescribable
    {
        /// <summary>
        /// A unique identifier for the type of equipment.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Class of equipment.
        /// </summary>
        public string EquipmentClass { get; set; }
        /// <summary>
        /// Standard of the equipment specification.
        /// </summary>
        public string Standard { get; set; }
        /// <summary>
        /// Reference to the standard of the equipment specification.
        /// </summary>
        public string StandardReference { get; set; }
    }
}