// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Query for listing containers.
    /// </summary>
    public class ContainersQuery : CursorQueryBase
    {
        /// <summary>
        /// The space to query
        /// </summary>
        public string Space { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var q = base.ToQueryParams();
            if (!string.IsNullOrEmpty(Space))
            {
                q.Add(("space", Space));
            }
            return q;
        }
    }
}
