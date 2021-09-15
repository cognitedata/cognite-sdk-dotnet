using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Update extraction pipeline.
    /// </summary>
    public class ExtPipeUpdate
    {
        /// <summary>
        /// External Id provided by client. Should be unique within the project, required.
        /// </summary>
        public Update<string> ExternalId { get; set; }
        /// <summary>
        /// Name of extraction pipeline, required. Up to 140 characters.
        /// </summary>
        public Update<string> Name { get; set; }
        /// <summary>
        /// Description of extraction pipeline. Up to 500 characters.
        /// </summary>
        public Update<string> Description { get; set; }
        /// <summary>
        /// Schedule of pipeline. Possible values: "On trigger", "Continuous", a cron expression, or empty.
        /// </summary>
        public Update<string> Schedule { get; set; }
        /// <summary>
        /// Optional list of associated raw tables.
        /// </summary>
        public UpdateEnumerable<ExtPipeRawTable> RawTables { get; set; }
        /// <summary>
        /// Optional list of contacts.
        /// </summary>
        public UpdateEnumerable<ExtPipeContact> Contacts { get; set; }
        /// <summary>
        /// Custom, application specific metadata. Keys are up to 128 bytes, Values up to 10240 bytes. Up to 256 key-value pairs.
        /// Total size is at most 10240 bytes.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }
        /// <summary>
        /// Source for extraction pipeline. Up to 255 characters.
        /// </summary>
        public Update<string> Source { get; set; }
        /// <summary>
        /// Long documentation text field, up to 10000 characters.
        /// </summary>
        public Update<string> Documentation { get; set; }
    }
}
