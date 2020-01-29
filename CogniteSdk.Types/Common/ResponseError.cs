// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// The DTO for errors received from CDF. Used for decoding API errors. Should not be used in user code. SDK will
    /// convert it directly to a ResponseException.
    /// </summary>
    public class ResponseErrorDto : Stringable
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
    }

    /// <summary>
    /// The DTO for errors received from CDF. Used for decoding API errors. Should not be used in user code. SDK will
    /// convert it directly to a ResponseException.
    /// </summary>
    public class ApiResponseErrorDto : Stringable
    {
        /// <summary>
        /// Response error object.
        /// </summary>
        public ResponseErrorDto Error { get; set; }

        /// <summary>
        /// Unique id for the request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Convert error to exception.
        /// </summary>
        public ResponseException ToException()
        {
            var exn = new ResponseException(this.Error.Message) {
                Code = this.Error.Code,
                Duplicated = this.Error.Duplicated,
                Missing = this.Error.Missing,
                RequestId = this.RequestId
            };

            return exn;
        }
    }
}
