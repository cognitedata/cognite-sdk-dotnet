// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Login
{
    /// <summary>
    /// The Login Data Read DTO.
    /// </summary>
    public class LoginDataReadDto : Stringable
    {
        /// <summary>
        /// Represents the current authentication status of the request
        /// </summary>
        public LoginStatusReadDto Data { get; set; }
    }
}