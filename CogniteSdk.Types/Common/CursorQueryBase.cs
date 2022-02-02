// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// A base class for queries with cursor and limits.
    /// </summary>
    public abstract class CursorQueryBase : IQueryParams
    {
        /// <summary>
        /// Limits the number of results to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Cursor to next result page.
        /// </summary>
        public string Cursor { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Stringable.ToString<CursorQueryBase>(this);
        }

        /// <inheritdoc/>
        public virtual List<(string, string)> ToQueryParams()
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