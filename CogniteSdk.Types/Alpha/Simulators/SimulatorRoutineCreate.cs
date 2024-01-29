

// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A Simulator routine to create
    /// </summary>
    public class SimulatorRoutineCreateCommandItem  {
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

    /// <summary>
    /// A Simulator routine to create of type predefined
    /// </summary>
    
    public class SimulatorRoutineCreateCommandPredefined  {
        /// <summary>
        /// External id provided by client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Model external id
        /// </summary>
        public string ModelExternalId { get; set; }

        /// <summary>
        /// Simulator Integratione external id.
        /// </summary>
        public string SimulatorIntegrationExternalId { get; set; }

        /// <summary>
        /// The calculation type of the routine
        /// The values must be one of the following : "IPR/VLP" "ChokeDp" "VLP" "IPR" "BhpFromRate" "BhpFromGradientTraverse" "BhpFromGaugeBhp"
        /// </summary>
        public string CalculationType { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

}