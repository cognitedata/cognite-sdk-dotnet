// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Raw
{
    public abstract class CursorQueryBase
    {
        /// <summary>
        /// Limits the number of results to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Cursor to next result page.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Convert query class to sequence of query parameter tuples.
        /// </summary>
        /// <returns>Key/value tuple sequence of all properties set in the query object.</returns>
        public List<(string, string)> ToQuery()
        {
            var list = new List<(string, string)>();
            if (Limit.HasValue)
                list.Add(("limit", Limit.ToString()));
            if (Cursor != null)
                list.Add(("cursor", Cursor));

            return list;
        }
    }
}