// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for listing groups
    /// </summary>
    public class GroupQuery : IQueryParams
    {
        /// <summary>
        /// True to list all groups. Default is false, which lists only the groups
        /// the current user is a member of.
        /// </summary>
        public bool All { get; set; }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var result = new List<(string, string)>();
            if (All)
            {
                result.Add(("all", "true"));
            }
            return result;
        }
    }
}
