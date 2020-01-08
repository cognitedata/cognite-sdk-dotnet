using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Common
{
    /// <summary>
    /// Holds several data items.
    /// </summary>
    /// <typeparam name="T">A resource type that is serializable.</typeparam>
    public class Items<T>
    {
        /// <summary>
        /// Data items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
