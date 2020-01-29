﻿using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// Query parameter when creating a raw table.
    /// </summary>
    public class RowCreateQuery : Stringable, IQueryParams
    {
        /// <summary>
        /// Create database/table if it doesn't exist already.
        /// </summary>
        public bool EnsureParent { get; set; }

        /// <inheritdoc/>
        public List<(string, string)> ToQueryParams()
        {
            return new List<(string, string)> { ("ensureParent", EnsureParent ? "true" : "false") };
        }
    }
}
