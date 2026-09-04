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
        /// <summary>
        /// Only include containers that have been marked as used for the specified purposes.
        /// Defaults to [node, edge, all]. The all value does not include record containers.
        /// </summary>
        public IEnumerable<UsedFor> UsedFor { get; set; }
        /// <summary>
        /// Whether to include global containers
        /// </summary>
        public bool IncludeGlobal { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var q = base.ToQueryParams();
            if (!string.IsNullOrEmpty(Space))
            {
                q.Add(("space", Space));
            }
            if (IncludeGlobal)
            {
                q.Add(("includeGlobal", "true"));
            }
            if (UsedFor != null)
            {
                foreach (var u in UsedFor)
                {
                    q.Add(("usedFor", u.ToString()));
                }
            }
            return q;
        }
    }
}
