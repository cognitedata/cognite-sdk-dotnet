using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Types.Raw
{
    public class Row
    {
        public string Key { get; set; }
        public IDictionary<string, JsonElement> Columns { get; set; }
        public long LastUpdatedTime { get; set; }
    }
}