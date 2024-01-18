

// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// Represents a command to create a simulator routine.
    /// </summary>
    public interface ISimulatorRoutineCreate { }

    /// <summary>
    /// A Simulator routine to create
    /// </summary>
    public class SimulatorRoutineCreateCommandItem : ISimulatorRoutineCreate {
        // <summary>
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

    }

    /// <summary>
    /// A Simulator routine to create of type predefined
    /// </summary>
    
    public class SimulatorRoutineCreateCommandPredefined : ISimulatorRoutineCreate {
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

    }


    /// <summary>
    /// Represents a command to create a simulator routine.
    /// </summary>
    /// <typeparam name="T">The type of the simulator routine create command.</typeparam>
    public class SimulatorRoutineCreate<T> where T : ISimulatorRoutineCreate
    {
        public T Value { get; set; }

        public SimulatorRoutineCreate(T value)
        {
            Value = value;
        }
    }
}