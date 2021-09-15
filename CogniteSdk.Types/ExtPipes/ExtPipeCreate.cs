using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Object used to create an extraction pipeline in CDF.
    /// </summary>
    public class ExtPipeCreate
    {
        /// <summary>
        /// External Id provided by client. Should be unique within the project, required.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Name of extraction pipeline, required. Up to 140 characters.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of extraction pipeline. Up to 500 characters.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// DataSet Id, required.
        /// </summary>
        public long DataSetId { get; set; }
        /// <summary>
        /// Optional list of associated raw tables.
        /// </summary>
        public IEnumerable<ExtPipeRawTable> RawTables { get; set; }
        /// <summary>
        /// Schedule of pipeline. Possible values: "On trigger", "Continuous", a cron expression, or empty.
        /// </summary>
        public string Schedule { get; set; }
        /// <summary>
        /// Optional list of contacts.
        /// </summary>
        public IEnumerable<ExtPipeContact> Contacts { get; set; }
        /// <summary>
        /// Custom, application specific metadata. Keys are up to 128 bytes, Values up to 10240 bytes. Up to 256 key-value pairs.
        /// Total size is at most 10240 bytes.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
        /// <summary>
        /// Source for extraction pipeline. Up to 255 characters.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Long documentation text field, up to 10000 characters.
        /// </summary>
        public string Documentation { get; set; }
    }
}
