// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Login
{
    /// <summary>
    /// The Login Data Read type.
    /// </summary>
    public class LoginDataRead
    {
        /// <summary>
        /// Represents the current authentication status of the request
        /// </summary>
        public LoginStatus Data { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<LoginDataRead>(this);
    }
}