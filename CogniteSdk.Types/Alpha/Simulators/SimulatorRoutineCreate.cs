

// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A Simulator routine to create
    /// </summary>
    public class SimulatorRoutineCreateCommandItem
    {
        /// <summary>
        /// External id provided by client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Model external id
        /// </summary>
        public string ModelExternalId { get; set; }

        /// <summary>
        /// Simulator integration external id.
        /// </summary>
        public string SimulatorIntegrationExternalId { get; set; }

        /// <summary>
        /// The name of the routine
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
