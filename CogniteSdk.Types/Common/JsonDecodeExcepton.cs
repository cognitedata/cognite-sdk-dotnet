// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// The main exception thrown by the SDK in case of errors.
    /// </summary>
    public class JsonDecodeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDecodeException">JsonDecodeException</see> class.
        /// </summary>
        public JsonDecodeException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDecodeException">JsonDecodeException</see> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public JsonDecodeException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDecodeException">JsonDecodeException</see> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception if any.</param>
        public JsonDecodeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
