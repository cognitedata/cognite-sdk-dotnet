using System.Collections.Generic;

namespace CogniteSdk.Types.Sequences
{
    public class Sequence
    {
        /// The Id of the sequence
        public long Id { get; set; }
        /// The name of the sequence.
        public string Name { get; set; }
        /// The description of the sequence.
        public string Description { get; set; }
        /// The valueType of the sequence. Enum STRING, DOUBLE, LONG
        public long AssetId { get; set; }
        /// The externalId of the sequence. Must be unique within the project.
        public string ExternalId { get; set; }
        /// Custom, application specific metadata. String key -> String value
        public IDictionary<string, string> MetaData { get; set; }
        /// Time when this sequence was created in CDF in milliseconds since Jan 1, 1970.
        public IEnumerable<Column> Columns { get; set; }
        /// Time when this sequence was created in CDF in milliseconds since Jan 1, 1970.
        public long CreatedTime { get; set; }
        /// The last time this sequence was updated in CDF, in milliseconds since Jan 1, 1970.
        public long LastUpdatedTime { get; set; }
    }
}
