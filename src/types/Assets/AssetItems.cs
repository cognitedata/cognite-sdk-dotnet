using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Assets
{
    public class AssetItems
    {
        public IEnumerable<Asset> Items { get; set; }
        public string NextCursor { get; set; }
    }
}


