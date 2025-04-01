// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevisionLog query class.
    /// </summary>
    public class ThreeDRevisionLogQuery : IQueryParams
    {
        /// <summary>
        /// Filter logs on severity.
        /// Minimum severity to retrieve (3 = INFO, 5 = WARN, 7 = ERROR).
        /// Default: 5
        /// </summary>
        public long? Severity { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<ThreeDRevisionLogQuery>(this);

        /// <inheritdoc/>
        public List<(string, string)> ToQueryParams()
        {
            var list = new List<(string, string)>();
            if (Severity.HasValue)
                list.Add(("severity", Severity.Value.ToString()));

            return list;
        }
    }
}
