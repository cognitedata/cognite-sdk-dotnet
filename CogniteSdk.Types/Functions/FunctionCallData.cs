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
        /// Data passed as argument to function.
        /// </summary>
        public T Data { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
