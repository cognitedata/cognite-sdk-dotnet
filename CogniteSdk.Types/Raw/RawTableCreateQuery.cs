using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query parameter when creating a raw table.
    /// </summary>
    public class RawTableCreateQuery : IQueryParams
    {
        /// <summary>
        /// Create database if it doesn't exist already.
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
