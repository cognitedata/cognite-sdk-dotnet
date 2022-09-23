// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Class for handling errors received from CDF. Used for decoding API errors. Should not be used in user code. SDK
    /// will convert it directly to a ResponseException.
    /// </summary>
    public class ResponseError
    {
        /// <summary>
        ///  The API error code (HTTP error code)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The missing entries if any.
        /// </summary>
        public IEnumerable<Dictionary<string, MultiValue>> Missing { get; set; }

        /// <summary>
        /// The duplicated entries if any.
        /// </summary>
        public IEnumerable<Dictionary<string, MultiValue>> Duplicated { get; set; }

        /// <summary>
        /// Complex extras object
        /// </summary>
        public JsonElement Extra { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The type for errors received from CDF. Used for decoding API errors. Should not be used in user code. SDK will
    /// convert it directly to a ResponseException.
    /// </summary>
    public class ApiResponseError
    {
        /// <summary>
        /// Response error object.
        /// </summary>
        public ResponseError Error { get; set; }

        /// <summary>
        /// Unique id for the request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Convert error to exception.
        /// </summary>
        public Exception ToException()
        {
            var exn = new ResponseException(this.Error.Message)
            {
                Code = this.Error.Code,
                Duplicated = this.Error.Duplicated,
                Missing = this.Error.Missing,
                RequestId = this.RequestId,
                Extra = this.Error.Extra
            };

            return exn;
        }

        /// <summary>
        /// Convert error to exception when ResponseError is missing.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="code">Exception http error code</param>
        /// <returns>Converted exception</returns>
        public Exception ToException(string message, int code)
        {
            return new ResponseException(message)
            {
                Code = code,
                RequestId = this.RequestId,
            };
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
