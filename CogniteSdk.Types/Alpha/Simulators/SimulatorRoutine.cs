// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// A Simulator routine .
    /// </summary>
    public class SimulatorRoutine {

        /// <summary>
        /// The unique identifier of the routine.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The external id of the routine.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The external id of the simulator.
        /// </summary>
        public string SimulatorExternalId { get; set; } 

        /// <summary>
        /// The external id of the model.
        /// </summary>
        public string ModelExternalId { get; set; }

        /// <summary>
        /// The external id of the simulator integration.
        /// </summary>
        public string SimulatorIntegrationExternalId { get; set; }

        /// <summary>
        /// The name of the routine.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the routine.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Calculation type. Only used for predefined routines.
        /// </summary>
        public string CalculationType { get; set; }

        /// <summary>
        /// The data set id of the routine.
        /// </summary>
        public long DataSetId { get; set; }

        /// <summary>
        /// The time when the routine was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The time when the routine was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }

    }
}
