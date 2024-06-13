// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The function call data class.
    /// </summary>
    public class FunctionCallData<T>
    {
        /// <summary>
        /// Nonce retrieved from sessions API when creating a session.
        /// This will be used to bind the session before executing the function.
        /// The corresponding access token will be passed to the function
        /// and used to instantiate the client of the handle() function.
        /// You can create a session via the Sessions API.
        /// When using the Python SDK, the session will be created behind
        /// the scenes when creating the schedule.
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// Data passed as argument to function.
        /// </summary>
        public T Data { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}