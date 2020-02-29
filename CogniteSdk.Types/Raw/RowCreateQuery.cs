using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Query parameter when creating a raw table.
    /// </summary>
    public class RowCreateQuery : IQueryParams
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
