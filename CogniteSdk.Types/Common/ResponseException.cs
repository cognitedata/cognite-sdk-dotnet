// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk
{
    /// <summary>
    /// The main exception thrown by the SDK in case of errors.
    /// </summary>
    public class ResponseException : Exception
    {
        /// <summary>
        /// HTTP error code.
        /// </summary>
        /// <value></value>
        public int Code { get; set; }

        /// <summary>
        /// Missing values.
        /// </summary>
        public IEnumerable<Dictionary<string, MultiValue>> Missing { get; set; }
        /// <summary>
        /// Duplicated values.
        /// </summary>
        public IEnumerable<Dictionary<string, MultiValue>> Duplicated { get; set; }

        /// <summary>
        /// Complex extras object
        /// </summary>
        public JsonElement Extra { get; set; }

        /// <summary>
        /// Request ID extracted from the response header.
        /// </summary>
        /// <value></value>
        public string RequestId { get; set; }

        /// <summary>
        /// Headers of the failed response, if any.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Set by the SDK when decoding the response")]
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public ResponseException() { }

        /// <summary>
        /// The response exception constructor.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>New response exception.</returns>
        public ResponseException(string message) : base(message) { }

        /// <summary>
        /// The response exception constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Original exception to be stored as inner exception.</param>
        /// <returns>New response exception.</returns>
        public ResponseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
