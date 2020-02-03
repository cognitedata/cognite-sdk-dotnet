// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Login
{
    /// <summary>
    /// The Login Data Read DTO.
    /// </summary>
    public class LoginDataReadDto
    {
        /// <summary>
        /// Represents the current authentication status of the request
        /// </summary>
        public LoginStatusReadDto Data { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<LoginDataReadDto>(this);
    }
}