// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Object used to deserialize the result of aggregate (count) requests.
    /// </summary>
    public class AggregateCount
    {
        /// <summary>
        /// Number of elements matching the query
        /// </summary>
        public int Count {get; set;}
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this.Count);
    }
}
