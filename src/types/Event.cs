using System.Collections.Generic;

namespace CogniteSdk.Types
{
    public class Event
    {
        public string ExternalId { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Description { get; set; }
        public IDictionary<string, string> MetaData { get; set; }
        public IEnumerable<long> AssetIds { get; set; }
        public string Source { get; set; }

        public long Id { get; set; }
        public long CreatedTime { get; set; }
        public long LastUpdatedTime { get; set; }
    }
}