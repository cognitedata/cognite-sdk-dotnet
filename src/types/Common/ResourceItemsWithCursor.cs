using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Common
{
    /// <summary>
    /// Resource items with a next cursor.
    /// </summary>
    /// <typeparam name="T">Resource type that is serializable.</typeparam>
    public class ResourceItemsWithCursor<T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Cursor to next page of data items.
        /// </summary>
        public string NextCursor { get; set; }
    }
}
