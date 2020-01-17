// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Abstract error value. Will either be LongValue, DoubleValue or StringValue.
    /// </summary>
    [JsonConverter(typeof(ErrorValue))]
    public abstract class ErrorValue {}

    /// <summary>
    /// Long (int64) error value
    /// </summary>
    public class LongValue : ErrorValue
    {
        public long Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    /// <summary>
    /// Double (float) error value. 
    /// </summary>
    public class DoubleValue : ErrorValue
    {
        public double Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    /// <summary>
    /// String error value.
    /// </summary>
    public class StringValue : ErrorValue
    {
        public string String { get; set; }

        public override string ToString()
        {
            return this.String;
        }
    }

    /// <summary>
    /// The DTO for errors received from CDF. Used for decoding API errors. Should not be used in user code as we will
    /// convert it directly to a ResponseException.
    /// </summary>
    public class ResponseErrorDto
    {
        /// <summary>
        ///  The API error code (HTTP error code)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }

        public IEnumerable<IDictionary<string, ErrorValue>> Missing;
        public IEnumerable<IDictionary<string, ErrorValue>> Duplicated;
    }

    /// <summary>
    /// The DTO for errors received from CDF. Used for decoding API errors. Should not be used in user code as we will
    /// convert it directly to a ResponseException.
    /// </summary>
    public class ApiResponseErrorDto
    {
        public ResponseErrorDto Error { get; set; }

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
        public IEnumerable<IDictionary<string, ErrorValue>> Missing;
        /// <summary>
        /// Duplicated values.
        /// </summary>
        public IEnumerable<IDictionary<string, ErrorValue>> Duplicated;

        /// <summary>
        /// Request ID extracted from the response header.
        /// </summary>
        /// <value></value>
        public string RequestId { get; set; }

        /// <summary>
        /// The response exception constructor.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>New response exception.</returns>
        public ResponseException(string message) : base(message) {}

        /// <summary>
        /// The response exception constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Original exception to be stored as inner exception.</param>
        /// <returns>New response exception.</returns>
        public ResponseException(string message, Exception innerException) : base(message, innerException) {}
    }
}
