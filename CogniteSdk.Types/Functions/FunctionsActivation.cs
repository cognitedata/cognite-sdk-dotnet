// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Signifies whether Cognite Functions have been activated
    /// for the project. Set to inactive by default.
    /// </summary>
    public enum FunctionsActivationStatus
    {
        /// <summary>
        /// Functions are not activated for this project.
        /// </summary>
        inactive,
        /// <summary>
        /// Function activation has been requested for this project,
        /// but is not yet completed.
        /// </summary>
        requested,
        /// <summary>
        /// Functions are activated for this project.
        /// </summary>
        activated
    }

    /// <summary>
    /// Response from the /functions/status endpoint.
    /// </summary>
    public class FunctionsActivationResponse
    {
        /// <summary>
        /// Status of function activation for the project.
        /// </summary>
        public FunctionsActivationStatus Status { get; set; }
    }
}