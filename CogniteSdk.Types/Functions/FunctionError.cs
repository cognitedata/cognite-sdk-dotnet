// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The FunctionError read class.
    /// </summary>
    public class FunctionError
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Stack trace of exception, useful for debugging.
        /// </summary>
        public string Trace { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
