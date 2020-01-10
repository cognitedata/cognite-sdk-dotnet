// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Common
{
    [JsonConverter(typeof(ErrorValue))]
    public abstract class ErrorValue {}

    public class LongValue : ErrorValue
    {
        public long Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    public class DoubleValue : ErrorValue
    {
        public double Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    public class StringValue : ErrorValue
    {
        public string String { get; set; }

        public override string ToString()
        {
            return this.String;
        }
    }

    public class ResponseError
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public IEnumerable<IDictionary<string, ErrorValue>> Missing;
        public IEnumerable<IDictionary<string, ErrorValue>> Duplicated;
    }

    public class ApiResponseError
    {
        public ResponseError Error { get; set; }

        public string RequestId { get; set; }

        /// <summary>
        /// Convert error to exception.
        /// </summary>
        public ResponseException ToException ()
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

    public class ResponseException : Exception
    {
        public int Code { get; set; }

        public IEnumerable<IDictionary<string, ErrorValue>> Missing;
        public IEnumerable<IDictionary<string, ErrorValue>> Duplicated;

        public string RequestId { get; set; }

        public ResponseException(string message) : base(message)
        {
        }

        public ResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
