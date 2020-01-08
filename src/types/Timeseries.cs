using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types
{
    public class TimeSeries
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsStep { get; set; }
        public bool IsString { get; set; }
        public string Unit { get; set; }

        public IDictionary<string, string> MetaData { get; set; }
        public IEnumerable<long> SecurityCategories { get; set; }
        public long Id { get; set; }
        public long AssetId { get; set; }
        public long CreatedTime { get; set; }
        public long LastUpdatedTime { get; set; }

        public string LegacyName { get; set; }
    }
}
