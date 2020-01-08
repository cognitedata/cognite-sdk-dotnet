using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Common
{
    /// <summary>
    /// Holds several resource items.
    /// </summary>
    /// <typeparam name="T">A resource type that is serializable.</typeparam>
    public class ResourceItems<T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
