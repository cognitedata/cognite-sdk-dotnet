using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Common
{
    public class ItemsWithCursor<T>
    {
        /// <summary>
        /// Items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Cursor to next page of data items.
        /// </summary>
        public string NextCursor { get; set; }
    }
}
