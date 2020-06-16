// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary    >
    /// The functionCallQuery read class.
    /// </summary>
    public class FunctionCallQuery
    {
        /// <summary>
        /// Filter for function call.
        /// </summary>
        /// <value></value>
        public FunctionCallFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

