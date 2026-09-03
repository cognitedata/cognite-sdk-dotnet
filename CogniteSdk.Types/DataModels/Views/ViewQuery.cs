// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Query for listing views
    /// </summary>
    public class ViewQuery : CursorQueryBase
    {
        /// <summary>
        /// The space to query.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Include properties inherited from views this view implements. Default is true.
        /// </summary>
        public bool IncludeInheritedProperties { get; set; } = true;
        /// <summary>
        /// If all versions of the view should be returned. Defaults to false which returns the latest version.
        /// </summary>
        public bool AllVersions { get; set; }
        /// <summary>
        /// Only include views that have been marked as used for the specified purposes.
        /// Defaults to [node, edge, all]. The all value does not include record views.
        /// </summary>
        public IEnumerable<UsedFor> UsedFor { get; set; }
        /// <summary>
        /// Whether to include global views.
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
            if (!IncludeInheritedProperties)
            {
                q.Add(("includeInheritedProperties", "false"));
            }
            if (AllVersions)
            {
                q.Add(("allVersions", "true"));
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

    /// <summary>
    /// Query for operations passing includeInheritedProperties.
    /// </summary>
    public class ViewIncludePropertiesQuery : IQueryParams
    {
        /// <summary>
        /// Include properties inherited from views this view implements. Default is true.
        /// </summary>
        public bool IncludeInheritedProperties { get; set; } = true;

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var q = new List<(string, string)>();
            if (!IncludeInheritedProperties)
            {
                q.Add(("includeInheritedProperties", "false"));
            }
            return q;
        }
    }
}
