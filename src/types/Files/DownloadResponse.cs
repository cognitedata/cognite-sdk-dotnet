using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Files
{
    public class DownloadResponse
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        public string DownloadUrl { get; set; }
    }
}
