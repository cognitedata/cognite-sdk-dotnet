

using System.Collections.Generic;

namespace CogniteSdk.Types.Assets
{
    public class AssetQuery
    {
        public AssetFilter filter { get; set; }
        public int Limit { get; set; }

        public string Cursor { get; set; }

        public IEnumerable<string> aggregatedProperties { get; set; }

        public string partition { get; set; }
    }
}