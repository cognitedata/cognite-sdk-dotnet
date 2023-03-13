// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels
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
